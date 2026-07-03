using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Core;
using Discoverer;

namespace task10tests;

public class ExtensionTests
{
    [Fact]
    public void Registrar_HasHook()
    {
        var type = typeof(Registrar.Registrar);
        var attr = type.GetCustomAttribute<HookAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void StorageService_HasHook()
    {
        var type = typeof(StorageService.StorageService);
        var attr = type.GetCustomAttribute<HookAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void ReportMaker_HasHook()
    {
        var type = typeof(ReportMaker.ReportMaker);
        var attr = type.GetCustomAttribute<HookAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void StorageService_AfterRegistrar()
    {
        var type = typeof(StorageService.StorageService);
        var attr = type.GetCustomAttribute<AfterAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(typeof(Registrar.Registrar), attr.Target);
    }

    [Fact]
    public void ReportMaker_AfterStorageService()
    {
        var type = typeof(ReportMaker.ReportMaker);
        var attr = type.GetCustomAttribute<AfterAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(typeof(StorageService.StorageService), attr.Target);
    }

    [Fact]
    public void Registrar_ImplementsIExecutable()
    {
        var type = typeof(Registrar.Registrar);
        Assert.True(typeof(IExecutable).IsAssignableFrom(type));
    }

    [Fact]
    public void StorageService_ImplementsIExecutable()
    {
        var type = typeof(StorageService.StorageService);
        Assert.True(typeof(IExecutable).IsAssignableFrom(type));
    }

    [Fact]
    public void ReportMaker_ImplementsIExecutable()
    {
        var type = typeof(ReportMaker.ReportMaker);
        Assert.True(typeof(IExecutable).IsAssignableFrom(type));
    }

    [Fact]
    public void Registrar_Run_WritesToConsole()
    {
        var obj = new Registrar.Registrar();
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);

        try
        {
            obj.Run();
            var output = writer.ToString();
            Assert.Contains("[REG] Регистратор активирован", output);
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public void StorageService_Run_WritesToConsole()
    {
        var obj = new StorageService.StorageService();
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);

        try
        {
            obj.Run();
            var output = writer.ToString();
            Assert.Contains("[STORAGE] Хранилище готово к работе", output);
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public void ReportMaker_Run_WritesToConsole()
    {
        var obj = new ReportMaker.ReportMaker();
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);

        try
        {
            obj.Run();
            var output = writer.ToString();
            Assert.Contains("[REPORT] Генератор отчётов запущен", output);
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public void Scan_ShouldFindExtensions()
    {
        var mgr = new ExtensionManager();
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Extensions", "Registrar", "bin", "Debug", "net8.0");

        if (Directory.Exists(dir))
        {
            mgr.Scan(dir);
            Assert.NotEmpty(mgr.Candidates);
        }
    }

    [Fact]
    public void BuildOrder_ShouldReturnCorrectOrder()
    {
        var mgr = new ExtensionManager();
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Extensions", "Registrar", "bin", "Debug", "net8.0");

        if (Directory.Exists(dir))
        {
            mgr.Scan(dir);
            var ordered = mgr.BuildOrder();

            int regIdx = -1, storIdx = -1, repIdx = -1;
            for (int i = 0; i < ordered.Count; i++)
            {
                if (ordered[i].Name == "Registrar") regIdx = i;
                if (ordered[i].Name == "StorageService") storIdx = i;
                if (ordered[i].Name == "ReportMaker") repIdx = i;
            }

            if (regIdx != -1 && storIdx != -1 && repIdx != -1)
            {
                Assert.True(regIdx < storIdx);
                Assert.True(storIdx < repIdx);
            }
        }
    }
}