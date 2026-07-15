using CommandLib;
using Core;

namespace StorageService;

[PluginLoad]
[PluginDependency(typeof(Registrar.Registrar))]
public class StorageService : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Хранилище загружено");
    }
}
