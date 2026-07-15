using task18;

namespace task19;

public sealed class HardStopAfterCommandsCommand : ILongRunningCommand
{
    private readonly IReadOnlyCollection<ILongRunningCommand> _commands;
    private readonly HardStopCommand _hardStopCommand;

    public HardStopAfterCommandsCommand(
        ServerThread serverThread,
        IEnumerable<ILongRunningCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(serverThread);
        ArgumentNullException.ThrowIfNull(commands);

        _commands = commands.ToArray();

        if (_commands.Count == 0)
        {
            throw new ArgumentException("At least one command is required.", nameof(commands));
        }

        _hardStopCommand = new HardStopCommand(serverThread);
    }

    public bool IsCompleted { get; private set; }

    public void Execute()
    {
        if (!_commands.All(command => command.IsCompleted))
        {
            return;
        }

        _hardStopCommand.Execute();
        IsCompleted = true;
    }
}
