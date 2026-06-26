using Xunit;
using System;
using System.Linq;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }
    private int PrivateProperty { get; set; }

    public void Method() { }
    public int MethodWithParams(string name, int age) => 0;
    private void PrivateMethod() { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("MethodWithParams", methods);
        Assert.DoesNotContain("PrivateMethod", methods);
    }

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(result);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var params1 = analyzer.GetMethodParams("MethodWithParams");

        Assert.Contains("String name", params1);
        Assert.Contains("Int32 age", params1);
        Assert.Equal(2, params1.Count());
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
        Assert.DoesNotContain("PrivateProperty", properties);
    }

    [Fact]
    public void GetMethodParams_WhenMethodNotFound_ReturnsEmpty()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("NonExistentMethod");

        Assert.Empty(result);
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeNotExists()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(result);
    }
}