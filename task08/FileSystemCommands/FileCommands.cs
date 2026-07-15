using System;
using System.IO;
using CommandLib;
using task07;

namespace FileSystemCommands;

[DisplayName("Проверка каталога")]
[Version(1, 0)]
public static class DirectoryHelper
{
    [DisplayName("Проверить существование каталога")]
    public static bool Exists(string path, out string error)
    {
        if (!Directory.Exists(path))
        {
            error = $"Директория не найдена: {path}";
            return false;
        }
        error = string.Empty;
        return true;
    }
}

[DisplayName("Вычисление размера каталога")]
[Version(1, 0)]
public class DirectorySizeCommand : ICommand
{
    private readonly string _targetPath;

    public DirectorySizeCommand(string targetPath)
    {
        _targetPath = targetPath;
    }

    [DisplayName("Выполнить вычисление размера")]
    public void Execute()
    {
        if (!DirectoryHelper.Exists(_targetPath, out string error))
        {
            Console.WriteLine(error);
            return;
        }

        long sizeInBytes = 0;
        foreach (string currentFile in Directory.GetFiles(_targetPath, "*", SearchOption.AllDirectories))
        {
            sizeInBytes += new FileInfo(currentFile).Length;
        }

        Console.WriteLine($"Общий размер директории '{_targetPath}': {sizeInBytes} байт ({sizeInBytes / 1024.0:F2} КБ)");
    }
}

[DisplayName("Поиск файлов по маске")]
[Version(1, 0)]
public class FindFilesCommand : ICommand
{
    private readonly string _searchPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string searchPath, string searchPattern)
    {
        _searchPath = searchPath;
        _searchPattern = searchPattern;
    }

    [DisplayName("Выполнить поиск файлов")]
    public void Execute()
    {
        if (!DirectoryHelper.Exists(_searchPath, out string error))
        {
            Console.WriteLine(error);
            return;
        }

        string[] matchedFiles = Directory.GetFiles(_searchPath, _searchPattern, SearchOption.AllDirectories);
        Console.WriteLine($"Найдено файлов по шаблону '{_searchPattern}': {matchedFiles.Length}");

        foreach (string filePath in matchedFiles)
        {
            Console.WriteLine($"  {filePath}");
        }
    }
}
