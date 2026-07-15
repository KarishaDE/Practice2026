using task18;

namespace task19;

public sealed class TestCommand : ILongRunningCommand
{
    private readonly int _id;
    private readonly int _executions;
    private readonly TextWriter _writer;

    public TestCommand(int id, int executions = 3, TextWriter? writer = null)
    {
        if (executions <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(executions));
        }

        _id = id;
        _executions = executions;
        _writer = writer ?? Console.Out;
    }

    public int Counter { get; private set; }

    public bool IsCompleted => Counter >= _executions;

    public void Execute()
    {
        if (IsCompleted)
        {
            return;
        }

        Counter++;
        _writer.WriteLine($"Поток {_id} вызов {Counter}");
    }
}
