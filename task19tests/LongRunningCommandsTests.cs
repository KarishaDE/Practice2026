using task18;
using task19;

namespace task19tests;

public class LongRunningCommandsTests
{
    [Fact]
    public void TestCommand_ThreeCalls_CompletesOperation()
    {
        using var writer = new StringWriter();
        var command = new TestCommand(1, writer: writer);

        command.Execute();
        command.Execute();
        command.Execute();

        Assert.True(command.IsCompleted);
        Assert.Equal(3, command.Counter);
        Assert.Equal(
            new[]
            {
                "Поток 1 вызов 1",
                "Поток 1 вызов 2",
                "Поток 1 вызов 3"
            },
            ReadLines(writer));
    }

    [Fact]
    public void TestCommand_AfterCompletion_DoesNotExecuteAgain()
    {
        using var writer = new StringWriter();
        var command = new TestCommand(2, writer: writer);

        command.Execute();
        command.Execute();
        command.Execute();
        command.Execute();

        Assert.Equal(3, command.Counter);
        Assert.Equal(3, ReadLines(writer).Length);
    }

    [Fact]
    public void FiveCommands_ExecuteThreeTimes_ThenHardStopServer()
    {
        using var writer = new StringWriter();
        var server = new ServerThread();
        var commands = Enumerable.Range(1, 5)
            .Select(id => new TestCommand(id, writer: writer))
            .ToArray();

        foreach (var command in commands)
        {
            server.AddCommand(command);
        }

        server.AddCommand(new HardStopAfterCommandsCommand(server, commands));
        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.False(server.IsAlive);
        Assert.All(commands, command =>
        {
            Assert.True(command.IsCompleted);
            Assert.Equal(3, command.Counter);
        });

        var expected = Enumerable.Range(1, 3)
            .SelectMany(call => Enumerable.Range(1, 5)
                .Select(id => $"Поток {id} вызов {call}"))
            .ToArray();

        Assert.Equal(expected, ReadLines(writer));
    }

    [Fact]
    public void TestCommand_CustomExecutionCount_IsSupported()
    {
        using var writer = new StringWriter();
        var command = new TestCommand(3, 2, writer);

        command.Execute();
        command.Execute();

        Assert.True(command.IsCompleted);
        Assert.Equal(2, command.Counter);
    }

    [Fact]
    public void TestCommand_InvalidExecutionCount_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestCommand(1, 0));
    }

    [Fact]
    public void HardStopAfterCommands_EmptyCollection_ThrowsException()
    {
        var server = new ServerThread();

        Assert.Throws<ArgumentException>(() =>
            new HardStopAfterCommandsCommand(server, Array.Empty<ILongRunningCommand>()));
    }

    private static string[] ReadLines(StringWriter writer)
    {
        return writer.ToString()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }
}
