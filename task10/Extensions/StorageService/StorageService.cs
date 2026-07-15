using System;
using Core;

namespace StorageService;

[Hook]
[After(typeof(Registrar.Registrar))]
public class StorageService : IExecutable
{
    public void Run()
    {
        Console.WriteLine("[STORAGE] Хранилище готово к работе");
    }
}