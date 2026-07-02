using System;
using System.Reflection;
using System.Linq;

class MetadataViewer
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: не указан путь к библиотеке");
            Console.WriteLine("Использование: dotnet run -- <путь_к_dll>");
            return;
        }

        string filePath = args[0];

        if (!System.IO.File.Exists(filePath))
        {
            Console.WriteLine($"Ошибка: файл '{filePath}' не существует");
            return;
        }

        try
        {
            Assembly loadedAssembly = Assembly.LoadFrom(filePath);
            AnalyzeAssembly(loadedAssembly);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки сборки: {ex.Message}");
        }
    }

    static void AnalyzeAssembly(Assembly assembly)
    {
        Console.WriteLine($"\nАнализ сборки: {assembly.GetName().Name}\n");

        var allTypes = assembly.GetTypes();
        var classTypes = allTypes.Where(t => t.IsClass && !t.IsAbstract).ToList();

        Console.WriteLine($"Найдено классов: {classTypes.Count}\n");

        foreach (var type in classTypes)
        {
            DisplayTypeDetails(type);
            Console.WriteLine(new string('-', 50));
        }
    }

    static void DisplayTypeDetails(Type type)
    {
        Console.WriteLine($"КЛАСС: {type.FullName}");

        var attributes = type.GetCustomAttributes();
        if (attributes.Any())
        {
            Console.WriteLine("  Атрибуты:");
            foreach (var attr in attributes)
            {
                Console.WriteLine($"    [{attr.GetType().Name}]");
            }
        }
        else
        {
            Console.WriteLine("  Атрибуты: отсутствуют");
        }

        Console.WriteLine("  Конструкторы:");
        var constructors = type.GetConstructors();
        if (constructors.Any())
        {
            foreach (var ctor in constructors)
            {
                DisplayMethodInfo(ctor);
            }
        }
        else
        {
            Console.WriteLine("    (конструкторы отсутствуют)");
        }

        Console.WriteLine("  Методы:");
        var methods = type.GetMethods();
        var filteredMethods = methods.Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"));
        if (filteredMethods.Any())
        {
            foreach (var method in filteredMethods)
            {
                DisplayMethodInfo(method);
            }
        }
        else
        {
            Console.WriteLine("    (методы отсутствуют)");
        }
    }

    static void DisplayMethodInfo(MethodBase methodInfo)
    {
        Console.WriteLine($"    {methodInfo.Name}()");

        var methodParams = methodInfo.GetParameters();
        if (methodParams.Length > 0)
        {
            Console.WriteLine("      Параметры:");
            foreach (var param in methodParams)
            {
                Console.WriteLine($"        {param.ParameterType.Name} {param.Name}");
            }
        }
        else
        {
            Console.WriteLine("      (без параметров)");
        }
    }
}