using System.Reflection;
using CommandLib;
using Core;
using Discoverer;

namespace task10tests;

public class PluginManagerTests
{
    [Fact]
    public void Plugins_HavePluginLoadAttribute()
    {
        var pluginTypes = new[]
        {
            typeof(Registrar.Registrar),
            typeof(StorageService.StorageService),
            typeof(ReportMaker.ReportMaker)
        };

        Assert.All(pluginTypes, type =>
            Assert.NotNull(type.GetCustomAttribute<PluginLoadAttribute>()));
    }

    [Fact]
    public void Discover_FindsAllPlugins()
    {
        var directory = CreatePluginDirectory();
        var manager = new PluginManager();

        try
        {
            var plugins = manager.Discover(directory);

            Assert.Equal(3, plugins.Count);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void BuildLoadOrder_ReturnsDependencyOrder()
    {
        var manager = CreateManagerWithPlugins(out var directory);

        try
        {
            var result = manager.BuildLoadOrder();

            Assert.Equal(
                new[] { "Registrar", "StorageService", "ReportMaker" },
                result.Select(type => type.Name));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void CreateCommands_CreatesCommandInstances()
    {
        var manager = CreateManagerWithPlugins(out var directory);

        try
        {
            var commands = manager.CreateCommands(manager.BuildLoadOrder());

            Assert.Equal(3, commands.Count);
            Assert.All(commands, command => Assert.IsAssignableFrom<ICommand>(command));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void ExecuteAll_ExecutesPluginsInDependencyOrder()
    {
        var manager = CreateManagerWithPlugins(out var directory);
        var writer = new StringWriter();
        var originalOutput = Console.Out;

        try
        {
            Console.SetOut(writer);
            var commands = manager.CreateCommands(manager.BuildLoadOrder());
            manager.ExecuteAll(commands);
            var output = writer.ToString();

            var registrarIndex = output.IndexOf("Регистратор", StringComparison.Ordinal);
            var storageIndex = output.IndexOf("Хранилище", StringComparison.Ordinal);
            var reportIndex = output.IndexOf("Генератор", StringComparison.Ordinal);

            Assert.True(registrarIndex >= 0);
            Assert.True(storageIndex >= 0);
            Assert.True(reportIndex >= 0);
            Assert.True(registrarIndex < storageIndex);
            Assert.True(storageIndex < reportIndex);
        }
        finally
        {
            Console.SetOut(originalOutput);
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void ResolveLoadOrder_WithCycle_ThrowsException()
    {
        var plugins = new[] { typeof(FirstCyclePlugin), typeof(SecondCyclePlugin) };

        Assert.Throws<InvalidOperationException>(() =>
            PluginManager.ResolveLoadOrder(plugins));
    }

    private static PluginManager CreateManagerWithPlugins(out string directory)
    {
        directory = CreatePluginDirectory();
        var manager = new PluginManager();
        manager.Discover(directory);
        return manager;
    }

    private static string CreatePluginDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"task10-{Guid.NewGuid()}");
        Directory.CreateDirectory(directory);

        var assemblies = new[]
        {
            typeof(Registrar.Registrar).Assembly,
            typeof(StorageService.StorageService).Assembly,
            typeof(ReportMaker.ReportMaker).Assembly
        };

        foreach (var assembly in assemblies)
        {
            File.Copy(
                assembly.Location,
                Path.Combine(directory, Path.GetFileName(assembly.Location)));
        }

        return directory;
    }
}

[PluginLoad]
[PluginDependency(typeof(SecondCyclePlugin))]
public class FirstCyclePlugin : ICommand
{
    public void Execute()
    {
    }
}

[PluginLoad]
[PluginDependency(typeof(FirstCyclePlugin))]
public class SecondCyclePlugin : ICommand
{
    public void Execute()
    {
    }
}
