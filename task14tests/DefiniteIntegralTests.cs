using task14;

namespace task14tests;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_LinearFunctionOnSymmetricInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, x => x, 1e-4, 2);

        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_SineOnSymmetricInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, Math.Sin, 1e-5, 8);

        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_LinearFunctionFromZeroToFive_ReturnsCorrectValue()
    {
        var result = DefiniteIntegral.Solve(0, 5, x => x, 1e-6, 8);

        Assert.Equal(12.5, result, 5);
    }

    [Fact]
    public void Solve_ConstantFunction_ReturnsRectangleArea()
    {
        var result = DefiniteIntegral.Solve(0, 5, _ => 2, 1e-4, 4);

        Assert.Equal(10, result, 5);
    }

    [Fact]
    public void Solve_OneAndEightThreads_ReturnSameResult()
    {
        var oneThreadResult = DefiniteIntegral.Solve(0, Math.PI, Math.Sin, 1e-5, 1);
        var eightThreadsResult = DefiniteIntegral.Solve(0, Math.PI, Math.Sin, 1e-5, 8);

        Assert.Equal(oneThreadResult, eightThreadsResult, 8);
    }

    [Fact]
    public void Solve_ReversedBounds_ReturnsNegativeValue()
    {
        var result = DefiniteIntegral.Solve(5, 0, x => x, 1e-5, 4);

        Assert.Equal(-12.5, result, 5);
    }

    [Fact]
    public void Solve_EqualBounds_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(2, 2, x => x * x, 1e-4, 3);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Solve_StepGreaterThanInterval_UsesOneTrapezoidPerSegment()
    {
        var result = DefiniteIntegral.Solve(0, 1, x => x, 10, 2);

        Assert.Equal(0.5, result, 10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Solve_InvalidStep_ThrowsException(double step)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, x => x, step, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Solve_InvalidThreadsNumber_ThrowsException(int threadsNumber)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, x => x, 1e-4, threadsNumber));
    }

    [Fact]
    public void Solve_NullFunction_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DefiniteIntegral.Solve(0, 1, null!, 1e-4, 2));
    }

    [Fact]
    public void Solve_FunctionThrows_PropagatesException()
    {
        Assert.Throws<AggregateException>(() =>
            DefiniteIntegral.Solve(0, 1, _ => throw new InvalidOperationException(), 1e-4, 4));
    }

    [Fact]
    public void Solve_NonFiniteBounds_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(double.NaN, 1, x => x, 1e-4, 2));
    }
}
