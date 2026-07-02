using Xunit;
using System;
using System.IO;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    private string CreateTempFolder()
    {
        string folderPath = Path.Combine(Path.GetTempPath(), "TestFolder_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_ShouldHandleMissingFolder()
    {
        string missingPath = Path.Combine(Path.GetTempPath(), "MissingFolder_" + Guid.NewGuid().ToString());
        var command = new DirectorySizeCommand(missingPath);
        Exception caughtException = Record.Exception(() => command.Execute());
        Assert.Null(caughtException);
    }

    [Fact]
    public void FindFilesCommand_ShouldHandleMissingFolder()
    {
        string missingPath = Path.Combine(Path.GetTempPath(), "MissingFolder_" + Guid.NewGuid().ToString());
        var command = new FindFilesCommand(missingPath, "*.txt");
        Exception caughtException = Record.Exception(() => command.Execute());
        Assert.Null(caughtException);
    }
}