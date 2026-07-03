using System;
using Core;

namespace Registrar;

[Hook]
public class Registrar : IExecutable
{
    public void Run()
    {
        Console.WriteLine("[REG] Регистратор активирован");
    }
}