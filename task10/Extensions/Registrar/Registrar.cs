using CommandLib;
using Core;

namespace Registrar;

[PluginLoad]
public class Registrar : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Регистратор загружен");
    }
}
