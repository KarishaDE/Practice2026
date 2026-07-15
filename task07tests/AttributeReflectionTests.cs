using System.Reflection;
using task07;

namespace task07tests;

public class ClassWithoutAttributes
{
}

public class AttributeReflectionTests
{
    [Fact]
    public void Class_HasDisplayNameAttribute()
    {
        var type = typeof(SampleClass);

        var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Пример класса", attribute.DisplayName);
    }

    [Fact]
    public void Method_HasDisplayNameAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");

        var attribute = method?.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Тестовый метод", attribute.DisplayName);
    }

    [Fact]
    public void Property_HasDisplayNameAttribute()
    {
        var property = typeof(SampleClass).GetProperty("Number");

        var attribute = property?.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Числовое свойство", attribute.DisplayName);
    }

    [Fact]
    public void Class_HasVersionAttribute()
    {
        var type = typeof(SampleClass);

        var attribute = type.GetCustomAttribute<VersionAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(1, attribute.Major);
        Assert.Equal(0, attribute.Minor);
    }

    [Fact]
    public void PrintTypeInfo_PrintsAllAttributeInformation()
    {
        var originalOutput = Console.Out;
        using var output = new StringWriter();

        try
        {
            Console.SetOut(output);
            ReflectionHelper.PrintTypeInfo(typeof(SampleClass));
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        var result = output.ToString();
        Assert.Contains("Отображаемое имя класса: Пример класса", result);
        Assert.Contains("Версия класса: 1.0", result);
        Assert.Contains("Метод TestMethod: Тестовый метод", result);
        Assert.Contains("Свойство Number: Числовое свойство", result);
    }

    [Fact]
    public void PrintTypeInfo_TypeWithoutAttributes_PrintsNothing()
    {
        var originalOutput = Console.Out;
        using var output = new StringWriter();

        try
        {
            Console.SetOut(output);
            ReflectionHelper.PrintTypeInfo(typeof(ClassWithoutAttributes));
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        Assert.Equal(string.Empty, output.ToString());
    }
}
