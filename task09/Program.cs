using task09;

if (args.Length == 0)
{
    Console.WriteLine("Не указан путь к библиотеке");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("Библиотека не найдена");
    return;
}

MetadataAnalyzer.Analyze(args[0], Console.Out);
