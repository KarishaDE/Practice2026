using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands;

public static class DirectoryHelper
{
    public static bool Exists(string path, out string error)
    {
        if (!Directory.Exists(path))
        {
            error = $"Директория не найдена: {path}";
            return false;
        }
        error = null;
        return true;
    }
}

public class DirectorySizeCommand : ICommand
{
    private readonly string _targetPath;

    public DirectorySizeCommand(string targetPath)
    {
        _targetPath = targetPath;
    }

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

public class FindFilesCommand : ICommand
{
    private readonly string _searchPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string searchPath, string searchPattern)
    {
        _searchPath = searchPath;
        _searchPattern = searchPattern;
    }

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