using System;
using Core;
using Discoverer;

class Program
{
    static void Main(string[] args)
    {
        var folder = args.Length > 0 ? args[0] : ".";

        var mgr = new ExtensionManager();

        try
        {
            mgr.Scan(folder);

            if (mgr.Candidates.Count == 0)
            {
                Console.WriteLine("Расширения не обнаружены");
                return;
            }

            Console.WriteLine("Обнаружены расширения:");
            foreach (var t in mgr.Candidates)
                Console.WriteLine($"  {t.FullName}");

            Console.WriteLine();

            var ordered = mgr.BuildOrder();

            Console.WriteLine("Порядок активации:");
            foreach (var t in ordered)
                Console.WriteLine($"  {t.FullName}");

            Console.WriteLine();

            var instances = mgr.Instantiate(ordered);
            mgr.ExecuteAll(instances);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}