using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;

    public int Property { get; set; }

    public void Method()
    {
    }

    public string MethodWithParams(string name, int age)
    {
        _privateField = name;
        return $"{_privateField}: {age}";
    }

    public static void StaticMethod()
    {
    }

    private void PrivateMethod()
    {
    }
}

[Serializable]
public class AttributedClass
{
}

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("MethodWithParams", methods);
        Assert.Contains("StaticMethod", methods);
        Assert.DoesNotContain("PrivateMethod", methods);
    }

    [Fact]
    public void GetMethodParams_ReturnsParameterNamesAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var parameters = analyzer.GetMethodParams("MethodWithParams");

        Assert.Equal(new[] { "name", "age", "String" }, parameters);
    }

    [Fact]
    public void GetMethodParams_WhenMethodDoesNotExist_ReturnsEmptyResult()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var parameters = analyzer.GetMethodParams("UnknownMethod");

        Assert.Empty(parameters);
    }

    [Fact]
    public void GetAllFields_IncludesPublicAndPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var fields = analyzer.GetAllFields();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsPropertyNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_WhenAttributeExists_ReturnsTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));

        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_WhenAttributeDoesNotExist_ReturnsFalse()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(result);
    }
}
