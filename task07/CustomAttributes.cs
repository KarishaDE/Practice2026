using System;
using System.Reflection;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public DisplayNameAttribute(string name)
    {
        DisplayName = name;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int majorVersion, int minorVersion)
    {
        Major = majorVersion;
        Minor = minorVersion;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }

    public void AnotherMethod() { }
}

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type currentType)
    {
        var display = currentType.GetCustomAttribute<DisplayNameAttribute>();
        if (display is not null)
        {
            Console.WriteLine($"Имя типа: {display.DisplayName}");
        }

        var version = currentType.GetCustomAttribute<VersionAttribute>();
        if (version is not null)
        {
            Console.WriteLine($"Версия: {version.Major}.{version.Minor}");
        }

        Console.WriteLine("\nСписок методов:");
        foreach (var m in currentType.GetMethods())
        {
            var a = m.GetCustomAttribute<DisplayNameAttribute>();
            if (a is not null)
            {
                Console.WriteLine($"  {a.DisplayName} (метод {m.Name})");
            }
        }

        Console.WriteLine("\nСписок свойств:");
        foreach (var p in currentType.GetProperties())
        {
            var a = p.GetCustomAttribute<DisplayNameAttribute>();
            if (a is not null)
            {
                Console.WriteLine($"  {a.DisplayName} (свойство {p.Name})");
            }
        }
    }
}