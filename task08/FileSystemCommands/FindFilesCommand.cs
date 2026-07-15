using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        var files = Directory
            .GetFiles(_directoryPath, _searchPattern, SearchOption.AllDirectories)
            .OrderBy(file => file)
            .ToList();

        Console.WriteLine($"Найдено файлов: {files.Count}");
        files.ForEach(Console.WriteLine);
    }
}
