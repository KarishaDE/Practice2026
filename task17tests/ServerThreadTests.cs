using task17;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsBeforeRemainingCommands()
    {
        var counter = 0;
        var server = new ServerThread();
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));
        server.AddCommand(new HardStopCommand(server));
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, counter);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void SoftStop_ExecutesAllRemainingCommands()
    {
        var counter = 0;
        var server = new ServerThread();
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));
        server.AddCommand(new SoftStopCommand(server));
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(3, counter);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void HardStop_ExecutedOutsideTargetThread_ThrowsException()
    {
        var server = new ServerThread();
        var command = new HardStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void SoftStop_ExecutedOutsideTargetThread_ThrowsException()
    {
        var server = new ServerThread();
        var command = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void EmptyQueue_WaitsForNewCommand()
    {
        using var executed = new ManualResetEventSlim(false);
        var server = new ServerThread();
        server.Start();

        Thread.Sleep(100);
        server.AddCommand(new ActionCommand(() => executed.Set()));
        server.AddCommand(new HardStopCommand(server));

        Assert.True(executed.Wait(TimeSpan.FromSeconds(2)));
        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void CommandException_IsPassedToHandlerAndProcessingContinues()
    {
        using var nextCommandExecuted = new ManualResetEventSlim(false);
        var handler = new TestExceptionHandler();
        var failedCommand = new ActionCommand(() => throw new InvalidOperationException("ошибка"));
        var server = new ServerThread(handler);
        server.AddCommand(failedCommand);
        server.AddCommand(new ActionCommand(() => nextCommandExecuted.Set()));
        server.AddCommand(new HardStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Same(failedCommand, handler.Command);
        Assert.IsType<InvalidOperationException>(handler.Exception);
        Assert.True(nextCommandExecuted.IsSet);
    }

    [Fact]
    public void UpdateBehavior_ChangesProcessingOfNextCommand()
    {
        var behaviorCalls = 0;
        var commandCalls = 0;
        var server = new ServerThread();
        server.UpdateBehavior(command =>
        {
            Interlocked.Increment(ref behaviorCalls);
            command.Execute();
        });
        server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref commandCalls)));
        server.AddCommand(new HardStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(2, behaviorCalls);
        Assert.Equal(1, commandCalls);
    }

    [Fact]
    public void AddCommand_AfterSoftStop_ThrowsException()
    {
        using var softStopExecuted = new ManualResetEventSlim(false);
        var server = new ServerThread();
        server.AddCommand(new ActionCommand(() => { }));
        server.AddCommand(new ActionCommand(() =>
        {
            new SoftStopCommand(server).Execute();
            softStopExecuted.Set();
        }));
        server.Start();

        Assert.True(softStopExecuted.Wait(TimeSpan.FromSeconds(2)));
        Assert.Throws<InvalidOperationException>(() =>
            server.AddCommand(new ActionCommand(() => { })));
        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void Start_CalledTwice_ThrowsException()
    {
        var server = new ServerThread();
        server.AddCommand(new HardStopCommand(server));
        server.Start();

        Assert.Throws<InvalidOperationException>(() => server.Start());
        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public async Task ConcurrentProducers_AllCommandsAreProcessed()
    {
        var counter = 0;
        var server = new ServerThread();
        server.Start();

        var producers = Enumerable.Range(0, 4)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < 25; i++)
                {
                    server.AddCommand(new ActionCommand(() => Interlocked.Increment(ref counter)));
                }
            }))
            .ToArray();

        await Task.WhenAll(producers);
        server.AddCommand(new SoftStopCommand(server));

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(100, counter);
    }

    private sealed class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }

    private sealed class TestExceptionHandler : IExceptionHandler
    {
        public ICommand? Command { get; private set; }

        public Exception? Exception { get; private set; }

        public void Handle(ICommand command, Exception exception)
        {
            Command = command;
            Exception = exception;
        }
    }
}
