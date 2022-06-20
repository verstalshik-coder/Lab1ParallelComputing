using System.Diagnostics;

namespace Lab2.resource
{
    internal class MyThreadPool :IDisposable
    {
        private readonly Thread[] _threads;
        private readonly ThreadPriority _priority;
        private readonly Queue<(Action<object?> Work, object? Parameter)> _works = new();
        private volatile bool _canWork = true;

        private readonly AutoResetEvent _workingEvent = new(false);
        private readonly AutoResetEvent _executeEvent = new(true);

        public MyThreadPool(int MaxThreadsCount, ThreadPriority priority = ThreadPriority.Normal)
        {
            if (MaxThreadsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(MaxThreadsCount), MaxThreadsCount, "Число потоков в пуле должно быть больше, либо равно 1");
            _priority = priority;
            _threads = new Thread[MaxThreadsCount];
            initialize();
        }

        private void initialize()
        {
            for (var i = 0; i < _threads.Length; i++)
            {
                var thread = new Thread(workingThread)
                {
                    Name = $"Thread{i}",
                    IsBackground = true,
                    Priority = _priority
                };
                _threads[i] = thread;
                thread.Start();
            }
        }

        public void execute(Action Work) => execute(null, _ => Work());

        public void execute(object? Parameter, Action<object?> Work)
        {
            if (!_canWork) throw new InvalidOperationException("Попытка передать задание уничтоженному пулу потоков");

            _executeEvent.WaitOne(); // запрашиваем доступ к очереди
            if (!_canWork) throw new InvalidOperationException("Попытка передать задание уничтоженному пулу потоков");

            _works.Enqueue((Work, Parameter));
            _executeEvent.Set();    // разрешаем доступ к очереди

            _workingEvent.Set();
        }

        private void workingThread()
        {
            var thread_name = Thread.CurrentThread.Name;

            try
            {
                while (_canWork)
                {
                    _workingEvent.WaitOne();
                    if (!_canWork) break;

                    _executeEvent.WaitOne(); // запрашиваем доступ к очереди

                    while (_works.Count == 0) // если (до тех пор пока) в очереди нет заданий
                    {
                        _executeEvent.Set(); // освобождаем очередь
                        _workingEvent.WaitOne(); // дожидаемся разрешения на выполнение
                        if (!_canWork) break;

                        _executeEvent.WaitOne(); // запрашиваем доступ к очереди вновь
                    }

                    var (work, parameter) = _works.Dequeue();
                    if (_works.Count > 0) // если после изъятия из очереди задания там осталось ещё что-то
                        _workingEvent.Set(); //  то запускаем ещё один поток на выполнение

                    _executeEvent.Set(); // разрешаем доступ к очереди

                    try
                    {
                        var timer = Stopwatch.StartNew();
                        work(parameter);
                        timer.Stop();

                        Trace.TraceInformation(
                            "Поток {0}[id:{1}] выполнил задание за {2}мс",
                            thread_name, Environment.CurrentManagedThreadId, timer.ElapsedMilliseconds);
                    }
                    catch (ThreadInterruptedException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Ошибка выполнения задания в потоке {0}:{1}", thread_name, e);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Trace.TraceWarning(
                    "Поток {0} был принудительно прерван при завершении работы пула",
                    thread_name);
            }
            finally
            {
                Trace.TraceInformation("Поток {0} завершил свою работу", thread_name);
                if (!_workingEvent.SafeWaitHandle.IsClosed)
                    _workingEvent.Set();
            }
        }

        private const int _DisposeThreadJoinTimeout = 100;
        public void Dispose()
        {
            _canWork = false;

            _workingEvent.Set();
            foreach (var thread in _threads)
                if (!thread.Join(_DisposeThreadJoinTimeout))
                    thread.Interrupt();

            _executeEvent.Dispose();
            _workingEvent.Dispose();
            Trace.TraceInformation("Пул потоков уничтожен");
        }
    }
}
