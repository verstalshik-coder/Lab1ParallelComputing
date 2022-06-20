using Lab1;
using Lab1.resource;

var result = new OneThread().start(FilePath.getFilePath());

Console.WriteLine("Result 1:");
Console.WriteLine($"Sum cictionary: {result.Item1}");
Console.WriteLine($"Time used: {result.Item2.TotalSeconds} sec");

Console.WriteLine();

result = new usingLINQ().start(FilePath.getFilePath());

Console.WriteLine("Result 2:");
Console.WriteLine($"Sum cictionary: {result.Item1}");
Console.WriteLine($"Time used: {result.Item2.TotalSeconds} sec");

Console.WriteLine();

result = new MultiThread().start(FilePath.getFilePath());

Console.WriteLine("Result 3:");
Console.WriteLine($"Sum cictionary: {result.Item1}");
Console.WriteLine($"Time used: {result.Item2.TotalSeconds} sec");