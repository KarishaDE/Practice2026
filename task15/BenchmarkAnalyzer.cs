namespace task15;

public static class BenchmarkAnalyzer
{
    public static double SelectSmallestAccurateStep(
        IEnumerable<StepMeasurement> measurements,
        double tolerance)
    {
        ArgumentNullException.ThrowIfNull(measurements);

        var accurateSteps = measurements
            .Where(measurement => measurement.Error <= tolerance)
            .Select(measurement => measurement.Step)
            .ToArray();

        if (accurateSteps.Length == 0)
        {
            throw new InvalidOperationException("Не найден шаг с требуемой точностью");
        }

        return accurateSteps.Min();
    }

    public static ThreadMeasurement SelectFastestThreadCount(
        IEnumerable<ThreadMeasurement> measurements)
    {
        ArgumentNullException.ThrowIfNull(measurements);

        return measurements
            .OrderBy(measurement => measurement.AverageMilliseconds)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Нет замеров количества потоков");
    }

    public static double CalculateSpeedupPercent(
        double singleThreadMilliseconds,
        double multiThreadMilliseconds)
    {
        if (singleThreadMilliseconds <= 0 || multiThreadMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(singleThreadMilliseconds),
                "Время должно быть положительным");
        }

        return (singleThreadMilliseconds - multiThreadMilliseconds) /
            singleThreadMilliseconds * 100.0;
    }
}
