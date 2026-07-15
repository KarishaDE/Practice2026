using System.Diagnostics;
using task14;

namespace task15;

public static class PerformanceResearch
{
    public const double Start = -100.0;
    public const double End = 100.0;
    public const double Tolerance = 1e-4;

    private static readonly double[] CandidateSteps =
    {
        1e-1,
        1e-2,
        1e-3,
        1e-4,
        1e-5,
        1e-6
    };

    public static ResearchSummary Run(
        string outputDirectory,
        int repetitions,
        int maxThreads)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException("Не указана папка результатов", nameof(outputDirectory));
        }

        if (repetitions < 3)
        {
            throw new ArgumentOutOfRangeException(
                nameof(repetitions),
                "Необходимо выполнить не менее трёх замеров");
        }

        if (maxThreads <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxThreads));
        }

        Directory.CreateDirectory(outputDirectory);
        WarmUp();

        var stepMeasurements = MeasureSteps(repetitions);
        var selectedStep = BenchmarkAnalyzer.SelectSmallestAccurateStep(
            stepMeasurements,
            Tolerance);

        var threadMeasurements = MeasureThreads(
            selectedStep,
            repetitions,
            maxThreads);

        var bestThreadMeasurement = BenchmarkAnalyzer.SelectFastestThreadCount(
            threadMeasurements);

        var singleThreadMeasurement = Measure(
            () => DefiniteIntegral.SolveSingleThread(Start, End, Math.Sin, selectedStep),
            repetitions);

        var singleThreadAverage = singleThreadMeasurement.TimesMilliseconds.Average();
        var speedupPercent = BenchmarkAnalyzer.CalculateSpeedupPercent(
            singleThreadAverage,
            bestThreadMeasurement.AverageMilliseconds);

        var summary = new ResearchSummary(
            selectedStep,
            stepMeasurements,
            threadMeasurements,
            bestThreadMeasurement,
            singleThreadMeasurement.Result,
            singleThreadMeasurement.TimesMilliseconds,
            speedupPercent);

        ResearchOutput.WriteAll(outputDirectory, summary);
        return summary;
    }

    private static IReadOnlyList<StepMeasurement> MeasureSteps(int repetitions)
    {
        var measurements = new List<StepMeasurement>();

        foreach (var step in CandidateSteps)
        {
            var measurement = Measure(
                () => DefiniteIntegral.Solve(Start, End, Math.Sin, step, 1),
                repetitions);

            var stepMeasurement = new StepMeasurement(
                step,
                measurement.Result,
                measurement.TimesMilliseconds);

            measurements.Add(stepMeasurement);
            Console.WriteLine(
                $"Шаг {step:E0}: ошибка {stepMeasurement.Error:E3}, " +
                $"среднее время {stepMeasurement.AverageMilliseconds:F3} мс");
        }

        return measurements;
    }

    private static IReadOnlyList<ThreadMeasurement> MeasureThreads(
        double step,
        int repetitions,
        int maxThreads)
    {
        var measurements = new List<ThreadMeasurement>();

        for (var threadCount = 1; threadCount <= maxThreads; threadCount++)
        {
            var currentThreadCount = threadCount;
            var measurement = Measure(
                () => DefiniteIntegral.Solve(
                    Start,
                    End,
                    Math.Sin,
                    step,
                    currentThreadCount),
                repetitions);

            var threadMeasurement = new ThreadMeasurement(
                currentThreadCount,
                measurement.Result,
                measurement.TimesMilliseconds);

            measurements.Add(threadMeasurement);
            Console.WriteLine(
                $"Потоков {currentThreadCount}: " +
                $"среднее время {threadMeasurement.AverageMilliseconds:F3} мс");
        }

        return measurements;
    }

    private static (double Result, IReadOnlyList<double> TimesMilliseconds) Measure(
        Func<double> calculation,
        int repetitions)
    {
        var times = new double[repetitions];
        var result = 0.0;

        for (var i = 0; i < repetitions; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            result = calculation();
            stopwatch.Stop();
            times[i] = stopwatch.Elapsed.TotalMilliseconds;
        }

        return (result, times);
    }

    private static void WarmUp()
    {
        DefiniteIntegral.SolveSingleThread(-1, 1, Math.Sin, 1e-3);
        DefiniteIntegral.Solve(-1, 1, Math.Sin, 1e-3, 2);
    }
}
