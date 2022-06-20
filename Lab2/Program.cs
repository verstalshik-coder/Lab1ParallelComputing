using Lab2.resource;

var numbers = Enumerable.Range(1, 4);

using (MyThreadPool thread_pool = new(2))
{    
    foreach (var num in numbers)
    {
        thread_pool.execute(num, obj =>
            {
                var temp = (int) obj!;
                Console.WriteLine(Math.Pow(temp, 2));
                Thread.Sleep(2000);
            });
    }

    Console.ReadLine();
}
