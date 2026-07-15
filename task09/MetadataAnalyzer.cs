using System.Reflection;

namespace task09;

public static class MetadataAnalyzer
{
    public static void Analyze(string libraryPath, TextWriter writer)
    {
        var assembly = Assembly.LoadFrom(libraryPath);
        var classes = assembly
            .GetTypes()
            .Where(type => type.IsClass)
            .OrderBy(type => type.FullName);

        writer.WriteLine($"Библиотека: {assembly.GetName().Name}");

        foreach (var type in classes)
        {
            PrintClass(type, writer);
        }
    }

    private static void PrintClass(Type type, TextWriter writer)
    {
        writer.WriteLine($"Класс: {type.FullName}");
        PrintAttributes(type, writer, "  ");

        writer.WriteLine("  Конструкторы:");
        var constructors = type.GetConstructors(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static);

        foreach (var constructor in constructors)
        {
            writer.WriteLine($"    Конструктор: {type.Name}");
            PrintParameters(constructor, writer, "      ");
        }

        writer.WriteLine("  Методы:");
        var methods = type.GetMethods(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            writer.WriteLine($"    Метод: {method.Name}");
            PrintAttributes(method, writer, "      ");
            PrintParameters(method, writer, "      ");
        }
    }

    private static void PrintAttributes(MemberInfo member, TextWriter writer, string indent)
    {
        writer.WriteLine($"{indent}Атрибуты:");

        foreach (var attribute in member.GetCustomAttributesData())
        {
            var arguments = attribute.ConstructorArguments
                .Select(FormatArgument);
            writer.WriteLine(
                $"{indent}  [{attribute.AttributeType.Name}({string.Join(", ", arguments)})]");
        }
    }

    private static void PrintParameters(MethodBase method, TextWriter writer, string indent)
    {
        foreach (var parameter in method.GetParameters())
        {
            writer.WriteLine(
                $"{indent}Параметр: {parameter.Name}, тип: {parameter.ParameterType.Name}");
        }
    }

    private static string FormatArgument(CustomAttributeTypedArgument argument)
    {
        return argument.Value switch
        {
            string value => $"\"{value}\"",
            null => "null",
            _ => argument.Value.ToString() ?? string.Empty
        };
    }
}
