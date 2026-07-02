using System;
using System.IO;
using System.Reflection;
using CommandLib;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск динамической загрузки команд...\n");

        string libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

        if (!File.Exists(libraryPath))
        {
            Console.WriteLine($"Ошибка: библиотека {libraryPath} не обнаружена!");
            Console.WriteLine("Убедитесь, что проект FileSystemCommands собран.");
            return;
        }

        Console.WriteLine($"Подключаем библиотеку: {libraryPath}\n");

        Assembly loadedAssembly = Assembly.LoadFrom(libraryPath);

        ExecuteCommand(loadedAssembly, "FileSystemCommands.DirectorySizeCommand",
            args.Length > 0 ? new object[] { args[0] } : new object[] { "." });

        ExecuteCommand(loadedAssembly, "FileSystemCommands.FindFilesCommand",
            args.Length > 0 ? new object[] { args[0], args.Length > 1 ? args[1] : "*.*" } : new object[] { ".", "*.*" });
    }

    private static void ExecuteCommand(Assembly assembly, string typeName, object[] constructorArgs)
    {
        Type commandType = assembly.GetType(typeName);
        if (commandType is not null)
        {
            Console.WriteLine($"Команда: {typeName.Split('.')[1]}");
            ICommand command = (ICommand)Activator.CreateInstance(commandType, constructorArgs);
            command.Execute();
            Console.WriteLine();
        }
    }
}