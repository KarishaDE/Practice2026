namespace task18;

public interface ILongRunningCommand : ICommand
{
    bool IsCompleted { get; }
}
