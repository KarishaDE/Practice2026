using System.Collections.Concurrent;

namespace task17;

public sealed class ServerThread
{
    private readonly BlockingCollection<ICommand> _commands = new();
    private readonly Thread _thread;
    private readonly IExceptionHandler? _exceptionHandler;
    private Action<ICommand> _behavior = command => command.Execute();
    private int _started;
    private int _stopMode;

    public ServerThread(IExceptionHandler? exceptionHandler = null)
    {
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
            throw new InvalidOperationException("Поток уже был запущен.");
        }

        _thread.Start();
    }

    public void AddCommand(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (Volatile.Read(ref _stopMode) != 0 || _commands.IsAddingCompleted)
        {
            throw new InvalidOperationException("Поток завершает работу.");
        }

        try
        {
            _commands.Add(command);
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Поток завершает работу.");
        }
    }

    public void UpdateBehavior(Action<ICommand> behavior)
    {
        ArgumentNullException.ThrowIfNull(behavior);
        Interlocked.Exchange(ref _behavior, behavior);
    }

    public bool Wait(TimeSpan timeout)
    {
        if (Volatile.Read(ref _started) == 0)
        {
            throw new InvalidOperationException("Поток еще не запущен.");
        }

        if (IsCurrentThread)
        {
            throw new InvalidOperationException("Поток не может ожидать сам себя.");
        }

        return _thread.Join(timeout);
    }

    internal bool IsCurrentThread => Thread.CurrentThread.ManagedThreadId == ManagedThreadId;

    internal void RequestHardStop()
    {
        EnsureCurrentThread();
        Interlocked.Exchange(ref _stopMode, 2);
        _commands.CompleteAdding();
    }

    internal void RequestSoftStop()
    {
        EnsureCurrentThread();
        Interlocked.CompareExchange(ref _stopMode, 1, 0);
        _commands.CompleteAdding();
    }

    private void ProcessCommands()
    {
        foreach (var command in _commands.GetConsumingEnumerable())
        {
            ExecuteCommand(command);

            if (Volatile.Read(ref _stopMode) == 2)
            {
                break;
            }
        }
    }

    private void ExecuteCommand(ICommand command)
    {
        try
        {
            Volatile.Read(ref _behavior)(command);
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
        }
    }

    private void EnsureCurrentThread()
    {
        if (!IsCurrentThread)
        {
            throw new InvalidOperationException("Команда остановки должна выполняться в целевом потоке.");
        }
    }
}
