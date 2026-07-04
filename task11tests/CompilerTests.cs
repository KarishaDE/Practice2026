using task11;

namespace task11tests;

public class CompilerTests
{
    [Theory]
    [InlineData(5, 3, 8)]
    [InlineData(-5, 3, -2)]
    [InlineData(0, 0, 0)]
    public void Execute_Add_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        var calc = DynamicCompiler.CompileAndCreate();
        var result = calc.Add(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 4, 6)]
    [InlineData(7, 7, 0)]
    [InlineData(0, 5, -5)]
    public void Execute_Minus_ShouldReturnCorrectDifference(int a, int b, int expected)
    {
        var calc = DynamicCompiler.CompileAndCreate();
        var result = calc.Minus(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(3, 4, 12)]
    [InlineData(-2, 3, -6)]
    [InlineData(0, 5, 0)]
    public void Execute_Mul_ShouldReturnCorrectProduct(int a, int b, int expected)
    {
        var calc = DynamicCompiler.CompileAndCreate();
        var result = calc.Mul(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(15, 3, 5)]
    [InlineData(8, 2, 4)]
    [InlineData(-6, 3, -2)]
    public void Execute_Div_ShouldReturnCorrectQuotient(int a, int b, int expected)
    {
        var calc = DynamicCompiler.CompileAndCreate();
        var result = calc.Div(a, b);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_DivByZero_ShouldThrowDivideByZero()
    {
        var calc = DynamicCompiler.CompileAndCreate();
        Assert.Throws<DivideByZeroException>(() => calc.Div(10, 0));
    }

    [Fact]
    public void Compiler_ShouldReturnNonNullInstance()
    {
        var calc = DynamicCompiler.CompileAndCreate();
        Assert.NotNull(calc);
    }
}
