using CommandLib;
using Core;

namespace ReportMaker;

[PluginLoad]
[PluginDependency(typeof(StorageService.StorageService))]
public class ReportMaker : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Генератор отчётов загружен");
    }
}
