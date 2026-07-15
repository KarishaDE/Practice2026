using System.Reflection;
using CommandLib;
using Core;

namespace Discoverer;

public class PluginManager
{
    private readonly List<Type> _plugins = new();

    public IReadOnlyList<Type> Plugins => _plugins;

    public IReadOnlyList<Type> Discover(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Каталог не найден: {directory}");
        }

        _plugins.Clear();

        var assemblies = Directory
            .GetFiles(directory, "*.dll")
            .OrderBy(path => path)
            .Select(TryLoadAssembly)
            .Where(assembly => assembly is not null)
            .Cast<Assembly>();

        _plugins.AddRange(assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsPlugin)
            .Distinct());

        return Plugins;
    }

    public IReadOnlyList<Type> BuildLoadOrder()
    {
        return ResolveLoadOrder(_plugins);
    }

    public static IReadOnlyList<Type> ResolveLoadOrder(IEnumerable<Type> plugins)
    {
        var pluginList = plugins.Distinct().OrderBy(type => type.FullName).ToList();
        var pluginSet = pluginList.ToHashSet();
        var states = new Dictionary<Type, int>();
        var result = new List<Type>();

        void Visit(Type plugin)
        {
            if (states.TryGetValue(plugin, out var state))
            {
                if (state == 1)
                {
                    throw new InvalidOperationException("Обнаружена циклическая зависимость плагинов");
                }

                return;
            }

            states[plugin] = 1;

            var dependencies = plugin
                .GetCustomAttributes<PluginDependencyAttribute>()
                .Select(attribute => attribute.PluginType);

            foreach (var dependency in dependencies)
            {
                if (!pluginSet.Contains(dependency))
                {
                    throw new InvalidOperationException(
                        $"Не найден плагин-зависимость: {dependency.FullName}");
                }

                Visit(dependency);
            }

            states[plugin] = 2;
            result.Add(plugin);
        }

        foreach (var plugin in pluginList)
        {
            Visit(plugin);
        }

        return result;
    }

    public IReadOnlyList<ICommand> CreateCommands(IEnumerable<Type> plugins)
    {
        return plugins
            .Select(type => (ICommand)Activator.CreateInstance(type)!)
            .ToList();
    }

    public void ExecuteAll(IEnumerable<ICommand> commands)
    {
        foreach (var command in commands)
        {
            command.Execute();
        }
    }

    private static Assembly? TryLoadAssembly(string path)
    {
        try
        {
            return Assembly.LoadFrom(path);
        }
        catch (BadImageFormatException)
        {
            return null;
        }
    }

    private static bool IsPlugin(Type type)
    {
        return type.IsClass
            && !type.IsAbstract
            && typeof(ICommand).IsAssignableFrom(type)
            && type.GetCustomAttribute<PluginLoadAttribute>() is not null
            && type.GetConstructor(Type.EmptyTypes) is not null;
    }
}
