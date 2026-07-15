using task14;
using task15;
using System.Text.Json;

namespace task15tests;

public class PerformanceResearchTests
{
    [Fact]
    public void SolveSingleThread_SineOnSymmetricInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.SolveSingleThread(-100, 100, Math.Sin, 1e-4);

        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void SolveSingleThread_AndSolve_ReturnEqualResults()
    {
        var single = DefiniteIntegral.SolveSingleThread(-10, 10, Math.Sin, 1e-4);
        var multi = DefiniteIntegral.Solve(-10, 10, Math.Sin, 1e-4, 4);

        Assert.Equal(single, multi, 8);
    }

    [Fact]
    public void SelectSmallestAccurateStep_ReturnsSmallestStep()
    {
        var measurements = new[]
        {
            CreateStepMeasurement(1e-2, 1e-5),
            CreateStepMeasurement(1e-4, 2e-5),
            CreateStepMeasurement(1e-6, 5e-5)
        };

        var result = BenchmarkAnalyzer.SelectSmallestAccurateStep(measurements, 1e-4);

        Assert.Equal(1e-6, result);
    }

    [Fact]
    public void SelectSmallestAccurateStep_WithoutAccurateValue_ThrowsException()
    {
        var measurements = new[]
        {
            CreateStepMeasurement(1e-2, 1e-2)
        };

        Assert.Throws<InvalidOperationException>(() =>
            BenchmarkAnalyzer.SelectSmallestAccurateStep(measurements, 1e-4));
    }

    [Fact]
    public void SelectFastestThreadCount_ReturnsLowestAverageTime()
    {
        var measurements = new[]
        {
            new ThreadMeasurement(1, 0, new[] { 100.0, 101.0, 99.0 }),
            new ThreadMeasurement(2, 0, new[] { 60.0, 61.0, 59.0 }),
            new ThreadMeasurement(4, 0, new[] { 70.0, 71.0, 69.0 })
        };

        var result = BenchmarkAnalyzer.SelectFastestThreadCount(measurements);

        Assert.Equal(2, result.ThreadCount);
    }

    [Fact]
    public void CalculateSpeedupPercent_ReturnsCorrectPercent()
    {
        var result = BenchmarkAnalyzer.CalculateSpeedupPercent(100, 70);

        Assert.Equal(30, result, 10);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(-1, 10)]
    public void CalculateSpeedupPercent_InvalidTime_ThrowsException(
        double single,
        double multi)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BenchmarkAnalyzer.CalculateSpeedupPercent(single, multi));
    }

    [Fact]
    public void SavedThreadMeasurements_ContainAllRuns()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "results",
            "thread-benchmarks.csv");
        var lines = File.ReadAllLines(path);

        Assert.Equal(17, lines.Length);
        Assert.Equal(
            "ThreadCount,Result,AbsoluteError,Run1_ms,Run2_ms,Run3_ms,Average_ms",
            lines[0]);
        Assert.All(lines.Skip(1), line => Assert.Equal(7, line.Split(',').Length));
    }

    [Fact]
    public void SavedOptimalResult_ConfirmsRequiredSpeedup()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "results",
            "optimal-result.txt");
        var text = File.ReadAllText(path);

        Assert.Contains("Оптимальное количество потоков: 15.", text);
        Assert.Contains("Требование ускорения не менее 15%: выполнено.", text);
    }

    [Fact]
    public void PolyglotNotebook_IsValidAndUsesScottPlot()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Task15Analysis.ipynb");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;

        Assert.Equal(4, root.GetProperty("nbformat").GetInt32());
        Assert.True(root.GetProperty("cells").GetArrayLength() >= 5);
        Assert.Contains("ScottPlot, 5.1.59", File.ReadAllText(path));
    }

    private static StepMeasurement CreateStepMeasurement(double step, double result)
    {
        return new StepMeasurement(step, result, new[] { 1.0, 1.0, 1.0 });
    }
}
