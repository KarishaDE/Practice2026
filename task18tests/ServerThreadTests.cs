using System.Collections.Concurrent;
using task18;

namespace task18tests;

public class ServerThreadTests
{
    [Fact]
    public void LongRunningCommands_AreExecutedRoundRobin()
    {
        var executionOrder = new ConcurrentQueue<string>();
        var first = new StepCommand("first", 3, executionOrder);
        var second = new StepCommand("second", 3, executionOrder);
        var server = new ServerThread();
        server.AddCommand(first);
        server.AddCommand(second);
        server.AddCommand(new SoftStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(
            new[] { "first", "second", "first", "second", "first", "second" },
            executionOrder);
        Assert.True(first.IsCompleted);
        Assert.True(second.IsCompleted);
    }

    [Fact]
    public void RegularCommand_RunsBeforeLongOperationFinishes()
    {
        var longCommand = new StepCommand("long", 5, new ConcurrentQueue<string>());
        var stepsAtRegularCommand = -1;
        var server = new ServerThread();
        server.AddCommand(longCommand);
        server.AddCommand(new ActionCommand(() => stepsAtRegularCommand = longCommand.ExecutedSteps));
        server.AddCommand(new SoftStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(0, stepsAtRegularCommand);
        Assert.Equal(5, longCommand.ExecutedSteps);
    }

    [Fact]
    public void NewCommand_RunsBetweenLongOperationSteps()
    {
        using var firstStepStarted = new ManualResetEventSlim(false);
        using var finishFirstStep = new ManualResetEventSlim(false);
        var longCommand = new ControlledStepCommand(firstStepStarted, finishFirstStep, 3);
        var stepsAtRegularCommand = -1;
        var server = new ServerThread();
        server.AddCommand(longCommand);
        server.Start();

        Assert.True(firstStepStarted.Wait(TimeSpan.FromSeconds(2)));
        server.AddCommand(new ActionCommand(() => stepsAtRegularCommand = longCommand.ExecutedSteps));
        server.AddCommand(new SoftStopCommand(server));
        finishFirstStep.Set();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, stepsAtRegularCommand);
        Assert.True(longCommand.IsCompleted);
    }

    [Fact]
    public void SoftStop_CompletesAllScheduledCommands()
    {
        var longCommand = new StepCommand("long", 4, new ConcurrentQueue<string>());
        var server = new ServerThread();
        server.AddCommand(longCommand);
        server.AddCommand(new SoftStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.True(longCommand.IsCompleted);
        Assert.Equal(4, longCommand.ExecutedSteps);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void HardStop_DoesNotCompleteScheduledCommands()
    {
        var longCommand = new StepCommand("long", 4, new ConcurrentQueue<string>());
        var server = new ServerThread();
        server.AddCommand(longCommand);
        server.AddCommand(new HardStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.False(longCommand.IsCompleted);
        Assert.Equal(0, longCommand.ExecutedSteps);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void EmptyServer_WaitsForNewCommand()
    {
        using var executed = new ManualResetEventSlim(false);
        var server = new ServerThread();
        server.Start();

        Thread.Sleep(100);
        server.AddCommand(new ActionCommand(() => executed.Set()));
        server.AddCommand(new SoftStopCommand(server));

        Assert.True(executed.Wait(TimeSpan.FromSeconds(2)));
        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void FailedLongCommand_IsNotScheduledAgain()
    {
        var handler = new TestExceptionHandler();
        var failedCommand = new FailingLongCommand();
        var server = new ServerThread(exceptionHandler: handler);
        server.AddCommand(failedCommand);
        server.AddCommand(new SoftStopCommand(server));

        server.Start();

        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, failedCommand.ExecuteCalls);
        Assert.Same(failedCommand, handler.Command);
        Assert.IsType<InvalidOperationException>(handler.Exception);
    }

    [Fact]
    public void StopCommands_ExecutedOutsideTargetThread_ThrowException()
    {
        var server = new ServerThread();

        Assert.Throws<InvalidOperationException>(() => new HardStopCommand(server).Execute());
        Assert.Throws<InvalidOperationException>(() => new SoftStopCommand(server).Execute());
    }

    [Fact]
    public void AddCommand_AfterSoftStop_ThrowsException()
    {
        using var stopped = new ManualResetEventSlim(false);
        var server = new ServerThread();
        server.AddCommand(new ActionCommand(() =>
        {
            new SoftStopCommand(server).Execute();
            stopped.Set();
        }));
        server.Start();

        Assert.True(stopped.Wait(TimeSpan.FromSeconds(2)));
        Assert.Throws<InvalidOperationException>(() =>
            server.AddCommand(new ActionCommand(() => { })));
        Assert.True(server.Wait(TimeSpan.FromSeconds(2)));
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

    private sealed class StepCommand : ILongRunningCommand
    {
        private readonly string _name;
        private readonly int _totalSteps;
        private readonly ConcurrentQueue<string> _executionOrder;

        public StepCommand(
            string name,
            int totalSteps,
            ConcurrentQueue<string> executionOrder)
        {
            _name = name;
            _totalSteps = totalSteps;
            _executionOrder = executionOrder;
        }

        public int ExecutedSteps { get; private set; }

        public bool IsCompleted => ExecutedSteps == _totalSteps;

        public void Execute()
        {
            _executionOrder.Enqueue(_name);
            ExecutedSteps++;
        }
    }

    private sealed class FailingLongCommand : ILongRunningCommand
    {
        public int ExecuteCalls { get; private set; }

        public bool IsCompleted => false;

        public void Execute()
        {
            ExecuteCalls++;
            throw new InvalidOperationException("failure");
        }
    }

    private sealed class ControlledStepCommand : ILongRunningCommand
    {
        private readonly ManualResetEventSlim _firstStepStarted;
        private readonly ManualResetEventSlim _finishFirstStep;
        private readonly int _totalSteps;

        public ControlledStepCommand(
            ManualResetEventSlim firstStepStarted,
            ManualResetEventSlim finishFirstStep,
            int totalSteps)
        {
            _firstStepStarted = firstStepStarted;
            _finishFirstStep = finishFirstStep;
            _totalSteps = totalSteps;
        }

        public int ExecutedSteps { get; private set; }

        public bool IsCompleted => ExecutedSteps == _totalSteps;

        public void Execute()
        {
            if (ExecutedSteps == 0)
            {
                _firstStepStarted.Set();
                _finishFirstStep.Wait(TimeSpan.FromSeconds(5));
            }

            ExecutedSteps++;
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
