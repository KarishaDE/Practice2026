using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using task18;
using task19;

var resultsDirectory = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "results"));

Directory.CreateDirectory(resultsDirectory);
WriteRoundRobinResults(resultsDirectory);
WriteCpuResults(resultsDirectory);

static void WriteRoundRobinResults(string resultsDirectory)
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

    if (!server.Wait(TimeSpan.FromSeconds(5)))
    {
        throw new TimeoutException("The command experiment did not finish in time.");
    }

    var lines = writer.ToString()
        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    var pattern = new Regex(@"Поток (\d+) вызов (\d+)");
    var csv = new List<string> { "order,command_id,call" };

    for (var index = 0; index < lines.Length; index++)
    {
        var match = pattern.Match(lines[index]);

        if (!match.Success)
        {
            throw new InvalidOperationException("Unexpected command output.");
        }

        csv.Add($"{index + 1},{match.Groups[1].Value},{match.Groups[2].Value}");
    }

    File.WriteAllLines(Path.Combine(resultsDirectory, "round_robin.csv"), csv);
}

static void WriteCpuResults(string resultsDirectory)
{
    const int runs = 5;
    var duration = TimeSpan.FromSeconds(1);
    MeasureIdle(TimeSpan.FromMilliseconds(100));
    MeasureBusy(TimeSpan.FromMilliseconds(100));

    var csv = new List<string> { "run,blocking_wait_ms,active_polling_ms" };

    for (var run = 1; run <= runs; run++)
    {
        var idle = MeasureIdle(duration);
        var busy = MeasureBusy(duration);
        csv.Add(string.Join(",",
            run,
            idle.ToString("F3", CultureInfo.InvariantCulture),
            busy.ToString("F3", CultureInfo.InvariantCulture)));
    }

    File.WriteAllLines(Path.Combine(resultsDirectory, "idle_cpu.csv"), csv);
}

static double MeasureIdle(TimeSpan duration)
{
    var process = Process.GetCurrentProcess();
    process.Refresh();
    var before = process.TotalProcessorTime;
    var server = new ServerThread();
    server.Start();
    Thread.Sleep(duration);
    server.AddCommand(new HardStopCommand(server));

    if (!server.Wait(TimeSpan.FromSeconds(5)))
    {
        throw new TimeoutException("The server did not stop in time.");
    }

    process.Refresh();
    return (process.TotalProcessorTime - before).TotalMilliseconds;
}

static double MeasureBusy(TimeSpan duration)
{
    var process = Process.GetCurrentProcess();
    process.Refresh();
    var before = process.TotalProcessorTime;
    var stopwatch = Stopwatch.StartNew();

    while (stopwatch.Elapsed < duration)
    {
        Thread.SpinWait(1000);
    }

    process.Refresh();
    return (process.TotalProcessorTime - before).TotalMilliseconds;
}
