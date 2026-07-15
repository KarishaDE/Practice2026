namespace task15;

public sealed record StepMeasurement(
    double Step,
    double Result,
    IReadOnlyList<double> TimesMilliseconds)
{
    public double Error => Math.Abs(Result);
    public double AverageMilliseconds => TimesMilliseconds.Average();
}

public sealed record ThreadMeasurement(
    int ThreadCount,
    double Result,
    IReadOnlyList<double> TimesMilliseconds)
{
    public double AverageMilliseconds => TimesMilliseconds.Average();
}

public sealed record ResearchSummary(
    double SelectedStep,
    IReadOnlyList<StepMeasurement> StepMeasurements,
    IReadOnlyList<ThreadMeasurement> ThreadMeasurements,
    ThreadMeasurement BestThreadMeasurement,
    double SingleThreadResult,
    IReadOnlyList<double> SingleThreadTimesMilliseconds,
    double SpeedupPercent)
{
    public double SingleThreadAverageMilliseconds =>
        SingleThreadTimesMilliseconds.Average();
}
