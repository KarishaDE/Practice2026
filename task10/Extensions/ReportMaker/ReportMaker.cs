using System;
using Core;

namespace ReportMaker;

[Hook]
[After(typeof(StorageService.StorageService))]
public class ReportMaker : IExecutable
{
    public void Run()
    {
        Console.WriteLine("[REPORT] Генератор отчётов запущен");
    }
}