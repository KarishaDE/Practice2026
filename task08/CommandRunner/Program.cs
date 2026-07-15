using CommandRunner;

var directoryPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
var searchPattern = args.Length > 1 ? args[1] : "*.*";
var libraryPath = Path.Combine(AppContext.BaseDirectory, "FileSystemCommands.dll");

var sizeCommand = CommandLoader.Load(
    libraryPath,
    "FileSystemCommands.DirectorySizeCommand",
    directoryPath);

var findCommand = CommandLoader.Load(
    libraryPath,
    "FileSystemCommands.FindFilesCommand",
    directoryPath,
    searchPattern);

sizeCommand.Execute();
findCommand.Execute();
