using System.Collections.Concurrent;

namespace task18;

public sealed class ServerThread
{
    private readonly ConcurrentQueue<ICommand> _commands = new();
    private readonly SemaphoreSlim _workAvailable = new(0);
    private readonly IScheduler _scheduler;
    private readonly IExceptionHandler? _exceptionHandler;
    private readonly Thread _thread;
    private readonly object _stateLock = new();
    private int _started;
    private int _stopMode;
    private bool _preferScheduled;

    public ServerThread(IScheduler? scheduler = null, IExceptionHandler? exceptionHandler = null)
    {
        _scheduler = scheduler ?? new RoundRobinScheduler();
        _exceptionHandler = exceptionHandler;
        _thread = new Thread(ProcessCommands)
        {
            IsBackground = true,
            Name = nameof(ServerThread)
        };
    }

    public bool IsAlive => _thread.IsAlive;

    public int ManagedThreadId => _thread.ManagedThreadId;

    public void Start()
    {
        if (Interlocked.CompareExchange(ref _started, 1, 0) != 0)
        {
            throw new InvalidOperationException("The thread has already been started.");
        }

        _thread.Start();
    }

    public void AddCommand(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        lock (_stateLock)
        {
            if (_stopMode != 0)
            {
                throw new InvalidOperationException("The thread is stopping.");
            }

            _commands.Enqueue(command);
            _workAvailable.Release();
        }
    }

    public bool Wait(TimeSpan timeout)
    {
        if (Volatile.Read(ref _started) == 0)
        {
            throw new InvalidOperationException("The thread has not been started.");
        }

        if (IsCurrentThread)
        {
            throw new InvalidOperationException("The thread cannot wait for itself.");
        }

        return _thread.Join(timeout);
    }

    internal bool IsCurrentThread => Thread.CurrentThread.ManagedThreadId == ManagedThreadId;

    internal void RequestHardStop()
    {
        EnsureCurrentThread();

        lock (_stateLock)
        {
            _stopMode = 2;
        }
    }

    internal void RequestSoftStop()
    {
        EnsureCurrentThread();

        lock (_stateLock)
        {
            if (_stopMode == 0)
            {
                _stopMode = 1;
            }
        }
    }

    private void ProcessCommands()
    {
        while (true)
        {
            if (Volatile.Read(ref _stopMode) == 2)
            {
                return;
            }

            if (_preferScheduled && TryExecuteScheduledCommand())
            {
                _preferScheduled = false;
                continue;
            }

            if (TryTakeNewCommand(out var command))
            {
                ProcessNewCommand(command);
                continue;
            }

            if (TryExecuteScheduledCommand())
            {
                _preferScheduled = false;
                continue;
            }

            if (Volatile.Read(ref _stopMode) == 1)
            {
                return;
            }

            _workAvailable.Wait();
        }
    }

    private bool TryTakeNewCommand(out ICommand command)
    {
        if (_commands.TryDequeue(out var queuedCommand))
        {
            _workAvailable.Wait(0);
            command = queuedCommand;
            return true;
        }

        command = null!;
        return false;
    }

    private void ProcessNewCommand(ICommand command)
    {
        if (command is ILongRunningCommand)
        {
            _scheduler.Add(command);
            _preferScheduled = _commands.IsEmpty;
            return;
        }

        ExecuteCommand(command);
        _preferScheduled = _scheduler.HasCommand();
    }

    private bool TryExecuteScheduledCommand()
    {
        if (!_scheduler.HasCommand())
        {
            return false;
        }

        var command = _scheduler.Select();
        var succeeded = ExecuteCommand(command);

        if (succeeded && command is ILongRunningCommand { IsCompleted: false })
        {
            _scheduler.Add(command);
        }

        return true;
    }

    private bool ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
            return true;
        }
        catch (Exception exception)
        {
            try
            {
                _exceptionHandler?.Handle(command, exception);
            }
            catch
            {
            }

            return false;
        }
    }

    private void EnsureCurrentThread()
    {
        if (!IsCurrentThread)
        {
            throw new InvalidOperationException("The stop command must run in the target thread.");
        }
    }
}
