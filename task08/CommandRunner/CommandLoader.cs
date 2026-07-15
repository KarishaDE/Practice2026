using System.Reflection;
using CommandLib;

namespace CommandRunner;

public static class CommandLoader
{
    public static ICommand Load(string libraryPath, string typeName, params object[] arguments)
    {
        var assembly = Assembly.LoadFrom(libraryPath);
        var commandType = assembly.GetType(typeName)
            ?? throw new InvalidOperationException($"Тип {typeName} не найден");

        if (!typeof(ICommand).IsAssignableFrom(commandType))
        {
            throw new InvalidOperationException($"Тип {typeName} не реализует ICommand");
        }

        return (ICommand)(Activator.CreateInstance(commandType, arguments)
            ?? throw new InvalidOperationException($"Не удалось создать {typeName}"));
    }
}
