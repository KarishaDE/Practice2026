using FileSystemCommands;
using task09;
using Xunit;

namespace task09tests;

public class MetadataAnalyzerTests
{
    [Fact]
    public void Analyze_PrintsAllClasses()
    {
        var result = AnalyzeCommandsLibrary();

        Assert.Contains("Класс: FileSystemCommands.DirectoryHelper", result);
        Assert.Contains("Класс: FileSystemCommands.DirectorySizeCommand", result);
        Assert.Contains("Класс: FileSystemCommands.FindFilesCommand", result);
    }

    [Fact]
    public void Analyze_PrintsAttributesAndTheirValues()
    {
        var result = AnalyzeCommandsLibrary();

        Assert.Contains(
            "[DisplayNameAttribute(\"Вычисление размера каталога\")]",
            result);
        Assert.Contains("[VersionAttribute(1, 0)]", result);
        Assert.Contains(
            "[DisplayNameAttribute(\"Выполнить поиск файлов\")]",
            result);
    }

    [Fact]
    public void Analyze_PrintsMethodsAndParameters()
    {
        var result = AnalyzeCommandsLibrary();

        Assert.Contains("Метод: Exists", result);
        Assert.Contains("Метод: Execute", result);
        Assert.Contains("Параметр: path, тип: String", result);
        Assert.Contains("Параметр: error, тип: String&", result);
    }

    [Fact]
    public void Analyze_PrintsConstructorsAndParameters()
    {
        var result = AnalyzeCommandsLibrary();

        Assert.Contains("Конструктор: DirectorySizeCommand", result);
        Assert.Contains("Параметр: targetPath, тип: String", result);
        Assert.Contains("Конструктор: FindFilesCommand", result);
        Assert.Contains("Параметр: searchPath, тип: String", result);
        Assert.Contains("Параметр: searchPattern, тип: String", result);
    }

    private static string AnalyzeCommandsLibrary()
    {
        using var writer = new StringWriter();
        var libraryPath = typeof(DirectorySizeCommand).Assembly.Location;

        MetadataAnalyzer.Analyze(libraryPath, writer);

        return writer.ToString();
    }
}
