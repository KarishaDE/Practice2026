using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core;

namespace Discoverer;

public class ExtensionManager
{
    private readonly List<Type> _candidates = new();
    private readonly Dictionary<Type, List<Type>> _graph = new();

    public IReadOnlyList<Type> Candidates => _candidates.AsReadOnly();

    public void Scan(string folder)
    {
        if (!Directory.Exists(folder))
            throw new Exception($"Папка не доступна: {folder}");

        foreach (var dll in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                foreach (var type in asm.GetTypes())
                {
                    if (IsEligible(type))
                    {
                        _candidates.Add(type);
                        _graph[type] = GetDependencies(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Пропуск {dll}: {ex.Message}");
            }
        }
    }

    public List<Type> BuildOrder()
    {
        var result = new List<Type>();
        var processed = new HashSet<Type>();

        void Process(Type type)
        {
            if (processed.Contains(type)) return;
            processed.Add(type);

            if (_graph.TryGetValue(type, out var deps))
            {
                foreach (var dep in deps)
                {
                    if (_candidates.Contains(dep))
                        Process(dep);
                }
            }

            result.Add(type);
        }

        foreach (var type in _candidates)
            Process(type);

        return result;
    }

    public List<IExecutable> Instantiate(List<Type> types)
    {
        var instances = new List<IExecutable>();
        foreach (var type in types)
        {
            try
            {
                var obj = (IExecutable)Activator.CreateInstance(type);
                instances.Add(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания {type.Name}: {ex.Message}");
            }
        }
        return instances;
    }

    public void ExecuteAll(List<IExecutable> items)
    {
        foreach (var item in items)
        {
            try
            {
                item.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении {item.GetType().Name}: {ex.Message}");
            }
        }
    }

    private bool IsEligible(Type type)
    {
        return type.IsClass && !type.IsAbstract &&
               typeof(IExecutable).IsAssignableFrom(type) &&
               type.GetCustomAttribute<HookAttribute>() != null;
    }

    private List<Type> GetDependencies(Type type)
    {
        var attrs = type.GetCustomAttributes<AfterAttribute>();
        var result = new List<Type>();
        foreach (var attr in attrs)
        {
            result.Add(attr.Target);
        }
        return result;
    }
}