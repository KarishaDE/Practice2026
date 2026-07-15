using CommandLib;
using CommandRunner;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDirectory = CreateTestDirectory();

        try
        {
            File.WriteAllBytes(Path.Combine(testDirectory, "test1.txt"), new byte[5]);
            File.WriteAllBytes(Path.Combine(testDirectory, "test2.txt"), new byte[7]);

            var result = ExecuteAndReadOutput(new DirectorySizeCommand(testDirectory));

            Assert.Contains("Размер каталога: 12 байт", result);
        }
        finally
        {
            Directory.Delete(testDirectory, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_ShouldIncludeSubdirectories()
    {
        var testDirectory = CreateTestDirectory();

        try
        {
            var subdirectory = Directory.CreateDirectory(
                Path.Combine(testDirectory, "subdirectory"));
            File.WriteAllBytes(Path.Combine(subdirectory.FullName, "file.txt"), new byte[8]);

            var result = ExecuteAndReadOutput(new DirectorySizeCommand(testDirectory));

            Assert.Contains("Размер каталога: 8 байт", result);
        }
        finally
        {
            Directory.Delete(testDirectory, true);
        }
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDirectory = CreateTestDirectory();

        try
        {
            var textFile = Path.Combine(testDirectory, "file1.txt");
            var logFile = Path.Combine(testDirectory, "file2.log");
            File.WriteAllText(textFile, "Text");
            File.WriteAllText(logFile, "Log");

            var result = ExecuteAndReadOutput(
                new FindFilesCommand(testDirectory, "*.txt"));

            Assert.Contains("Найдено файлов: 1", result);
            Assert.Contains(textFile, result);
            Assert.DoesNotContain(logFile, result);
        }
        finally
        {
            Directory.Delete(testDirectory, true);
        }
    }

    [Fact]
    public void CommandLoader_ShouldLoadCommandFromLibrary()
    {
        var libraryPath = typeof(DirectorySizeCommand).Assembly.Location;

        var command = CommandLoader.Load(
            libraryPath,
            "FileSystemCommands.DirectorySizeCommand",
            Path.GetTempPath());

        Assert.IsType<DirectorySizeCommand>(command);
        Assert.IsAssignableFrom<ICommand>(command);
    }

    private static string CreateTestDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"task08_{Guid.NewGuid()}");
        Directory.CreateDirectory(path);
        return path;
    }

    private static string ExecuteAndReadOutput(ICommand command)
    {
        var originalOutput = Console.Out;
        using var output = new StringWriter();

        try
        {
            Console.SetOut(output);
            command.Execute();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        return output.ToString();
    }
}
