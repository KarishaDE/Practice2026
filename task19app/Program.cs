using task18;
using task19;

var server = new ServerThread();
var commands = Enumerable.Range(1, 5)
    .Select(id => new TestCommand(id))
    .ToArray();

foreach (var command in commands)
{
    server.AddCommand(command);
}

server.AddCommand(new HardStopAfterCommandsCommand(server, commands));
server.Start();

if (!server.Wait(TimeSpan.FromSeconds(5)))
{
    throw new TimeoutException("The demonstration did not finish in time.");
}
