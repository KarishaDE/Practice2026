using task15;

var repetitions = args.Length > 0 ? int.Parse(args[0]) : 3;
var maxThreads = args.Length > 1 ? int.Parse(args[1]) : Environment.ProcessorCount;
var outputDirectory = args.Length > 2
    ? args[2]
    : Path.Combine(Environment.CurrentDirectory, "task15", "results");

var summary = PerformanceResearch.Run(outputDirectory, repetitions, maxThreads);

Console.WriteLine();
Console.WriteLine($"Выбранный шаг: {summary.SelectedStep:E0}");
Console.WriteLine($"Оптимальное число потоков: {summary.BestThreadMeasurement.ThreadCount}");
Console.WriteLine($"Однопоточное время: {summary.SingleThreadAverageMilliseconds:F3} мс");
Console.WriteLine($"Многопоточное время: {summary.BestThreadMeasurement.AverageMilliseconds:F3} мс");
Console.WriteLine($"Ускорение: {summary.SpeedupPercent:F2}%");
Console.WriteLine($"Результаты сохранены: {Path.GetFullPath(outputDirectory)}");
