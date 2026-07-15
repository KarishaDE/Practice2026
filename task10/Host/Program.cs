using Discoverer;

var directory = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "Plugins");

try
{
    var manager = new PluginManager();
    var plugins = manager.Discover(directory);

    if (plugins.Count == 0)
    {
        Console.WriteLine("Плагины не найдены");
        return;
    }

    var loadOrder = manager.BuildLoadOrder();

    Console.WriteLine("Порядок загрузки плагинов:");
    foreach (var plugin in loadOrder)
    {
        Console.WriteLine(plugin.FullName);
    }

    var commands = manager.CreateCommands(loadOrder);
    manager.ExecuteAll(commands);
}
catch (Exception exception)
{
    Console.WriteLine($"Ошибка: {exception.Message}");
}
