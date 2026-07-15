using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayName is not null)
        {
            Console.WriteLine($"Отображаемое имя класса: {displayName.DisplayName}");
        }

        var version = type.GetCustomAttribute<VersionAttribute>();
        if (version is not null)
        {
            Console.WriteLine($"Версия класса: {version.Major}.{version.Minor}");
        }

        var methods = type.GetMethods(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
            if (attribute is not null)
            {
                Console.WriteLine($"Метод {method.Name}: {attribute.DisplayName}");
            }
        }

        var properties = type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<DisplayNameAttribute>();
            if (attribute is not null)
            {
                Console.WriteLine($"Свойство {property.Name}: {attribute.DisplayName}");
            }
        }
    }
}
