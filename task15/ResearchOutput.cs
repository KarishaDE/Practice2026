using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using ScottPlot;

namespace task15;

public static class ResearchOutput
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public static void WriteAll(string outputDirectory, ResearchSummary summary)
    {
        WriteStepCsv(Path.Combine(outputDirectory, "step-benchmarks.csv"), summary);
        WriteThreadCsv(Path.Combine(outputDirectory, "thread-benchmarks.csv"), summary);
        WriteOptimalResult(Path.Combine(outputDirectory, "optimal-result.txt"), summary);
        WriteReport(Path.Combine(outputDirectory, "REPORT.md"), summary);
        WriteThreadPlot(Path.Combine(outputDirectory, "thread-performance.png"), summary);
        WriteComparisonPlot(Path.Combine(outputDirectory, "single-vs-multi.png"), summary);
    }

    private static void WriteStepCsv(string path, ResearchSummary summary)
    {
        var builder = new StringBuilder();
        WriteHeader(builder, "Step", summary.StepMeasurements[0].TimesMilliseconds.Count);

        foreach (var measurement in summary.StepMeasurements)
        {
            builder.Append(measurement.Step.ToString("E0", Invariant));
            builder.Append(',');
            builder.Append(measurement.Result.ToString("G17", Invariant));
            builder.Append(',');
            builder.Append(measurement.Error.ToString("G17", Invariant));
            AppendTimes(builder, measurement.TimesMilliseconds);
        }

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    private static void WriteThreadCsv(string path, ResearchSummary summary)
    {
        var builder = new StringBuilder();
        WriteHeader(builder, "ThreadCount", summary.ThreadMeasurements[0].TimesMilliseconds.Count);

        foreach (var measurement in summary.ThreadMeasurements)
        {
            builder.Append(measurement.ThreadCount);
            builder.Append(',');
            builder.Append(measurement.Result.ToString("G17", Invariant));
            builder.Append(',');
            builder.Append(Math.Abs(measurement.Result).ToString("G17", Invariant));
            AppendTimes(builder, measurement.TimesMilliseconds);
        }

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    private static void WriteHeader(StringBuilder builder, string parameterName, int repetitions)
    {
        builder.Append(parameterName);
        builder.Append(",Result,AbsoluteError");

        for (var i = 1; i <= repetitions; i++)
        {
            builder.Append($",Run{i}_ms");
        }

        builder.AppendLine(",Average_ms");
    }

    private static void AppendTimes(
        StringBuilder builder,
        IReadOnlyList<double> timesMilliseconds)
    {
        foreach (var time in timesMilliseconds)
        {
            builder.Append(',');
            builder.Append(time.ToString("F6", Invariant));
        }

        builder.Append(',');
        builder.AppendLine(timesMilliseconds.Average().ToString("F6", Invariant));
    }

    private static void WriteOptimalResult(string path, ResearchSummary summary)
    {
        var text = $$"""
            Исследуемая функция: sin(x).
            Границы интегрирования: от -100 до 100.
            Требуемая точность: 1e-4.
            Выбранный минимальный размер шага: {{summary.SelectedStep:E0}}.
            Оптимальное количество потоков: {{summary.BestThreadMeasurement.ThreadCount}}.
            Среднее время однопоточной реализации без создания потоков: {{summary.SingleThreadAverageMilliseconds:F3}} мс.
            Среднее время оптимальной многопоточной реализации: {{summary.BestThreadMeasurement.AverageMilliseconds:F3}} мс.
            Ускорение многопоточной реализации относительно однопоточной: {{summary.SpeedupPercent:F2}}%.
            Требование ускорения не менее 15%: {{(summary.SpeedupPercent >= 15 ? "выполнено" : "не выполнено")}}.
            Количество повторов каждого замера: {{summary.SingleThreadTimesMilliseconds.Count}}.
            Число логических процессоров: {{Environment.ProcessorCount}}.
            """;

        File.WriteAllText(path, text, Encoding.UTF8);
    }

    private static void WriteReport(string path, ResearchSummary summary)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Отчёт по задаче 15");
        builder.AppendLine();
        builder.AppendLine("## Условия исследования");
        builder.AppendLine();
        builder.AppendLine("Вычислялся определённый интеграл функции `sin(x)` на отрезке `[-100; 100]` методом трапеций. Теоретическое значение интеграла равно нулю, поскольку функция нечётная, а отрезок симметричен. Допустимая абсолютная погрешность равна `1e-4`.");
        builder.AppendLine();
        builder.AppendLine($"Каждый замер выполнен {summary.SingleThreadTimesMilliseconds.Count} раза. Для измерения применён `Stopwatch`, итоговое время вычислено как среднее арифметическое. Среда: {RuntimeInformation.FrameworkDescription}, {RuntimeInformation.OSDescription}, логических процессоров: {Environment.ProcessorCount}.");
        builder.AppendLine();
        builder.AppendLine("## Выбор шага");
        builder.AppendLine();
        builder.AppendLine("| Шаг | Результат | Абсолютная ошибка | Среднее время, мс |");
        builder.AppendLine("|---:|---:|---:|---:|");

        foreach (var measurement in summary.StepMeasurements)
        {
            builder.AppendLine($"| `{measurement.Step:E0}` | `{measurement.Result:E6}` | `{measurement.Error:E6}` | `{measurement.AverageMilliseconds:F3}` |");
        }

        builder.AppendLine();
        builder.AppendLine($"Все исследованные шаги обеспечили требуемую точность. Это связано с нечётностью `sin(x)` и симметрией отрезка. В соответствии с формулировкой о минимальном размере из заданного набора выбран шаг `{summary.SelectedStep:E0}`. Он создаёт достаточную вычислительную нагрузку для объективного исследования многопоточности.");
        builder.AppendLine();
        builder.AppendLine("## Выбор количества потоков");
        builder.AppendLine();
        builder.AppendLine("| Потоки | Результат | Среднее время, мс |");
        builder.AppendLine("|---:|---:|---:|");

        foreach (var measurement in summary.ThreadMeasurements)
        {
            builder.AppendLine($"| {measurement.ThreadCount} | `{measurement.Result:E6}` | `{measurement.AverageMilliseconds:F3}` |");
        }

        builder.AppendLine();
        builder.AppendLine($"Минимальное среднее время получено при использовании **{summary.BestThreadMeasurement.ThreadCount} потоков**: **{summary.BestThreadMeasurement.AverageMilliseconds:F3} мс**.");
        builder.AppendLine();
        builder.AppendLine("График `thread-performance.png` построен программно библиотекой ScottPlot. По оси OX указано среднее время выполнения `Solve`, по оси OY — количество потоков.");
        builder.AppendLine();
        builder.AppendLine("## Сравнение с однопоточной реализацией");
        builder.AppendLine();
        builder.AppendLine("Однопоточный вариант реализован отдельным методом `SolveSingleThread` и не создаёт потоки. Это исключает накладные расходы `Thread`, `Barrier` и `Interlocked`.");
        builder.AppendLine();
        builder.AppendLine($"- однопоточная реализация: **{summary.SingleThreadAverageMilliseconds:F3} мс**;");
        builder.AppendLine($"- лучшая многопоточная реализация: **{summary.BestThreadMeasurement.AverageMilliseconds:F3} мс**;");
        builder.AppendLine($"- преимущество многопоточной реализации: **{summary.SpeedupPercent:F2}%**.");
        builder.AppendLine();
        builder.AppendLine(summary.SpeedupPercent >= 15
            ? "Требование о преимуществе не менее 15% выполнено."
            : "Требование о преимуществе не менее 15% не выполнено; требуется дополнительная оптимизация.");
        builder.AppendLine();
        builder.AppendLine("## Вывод");
        builder.AppendLine();
        builder.AppendLine("Применение нескольких потоков оправдано для малого шага и большого числа разбиений, поскольку вычислительная нагрузка существенно превышает затраты на создание и синхронизацию потоков. Увеличение числа потоков после оптимального значения не гарантирует дальнейшего ускорения из-за планирования потоков, конкуренции за вычислительные ресурсы и изменения частоты процессора.");

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    private static void WriteThreadPlot(string path, ResearchSummary summary)
    {
        var plot = new Plot();
        var times = summary.ThreadMeasurements
            .Select(measurement => measurement.AverageMilliseconds)
            .ToArray();
        var threads = summary.ThreadMeasurements
            .Select(measurement => (double)measurement.ThreadCount)
            .ToArray();

        plot.Add.Scatter(times, threads);
        plot.Title("Solve performance by thread count");
        plot.XLabel("Average time, ms");
        plot.YLabel("Thread count");
        plot.SavePng(path, 1000, 650);
    }

    private static void WriteComparisonPlot(string path, ResearchSummary summary)
    {
        var plot = new Plot();
        var values = new[]
        {
            summary.SingleThreadAverageMilliseconds,
            summary.BestThreadMeasurement.AverageMilliseconds
        };

        var bars = plot.Add.Bars(values);
        bars.Bars[0].Label = $"{values[0]:F1} ms";
        bars.Bars[1].Label = $"{values[1]:F1} ms";
        bars.ValueLabelStyle.Bold = true;

        Tick[] ticks =
        {
            new(0, "Single-thread"),
            new(1, $"Multi-thread ({summary.BestThreadMeasurement.ThreadCount})")
        };

        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
        plot.Axes.Margins(bottom: 0, top: 0.2);
        plot.Title("Single-thread and multi-thread comparison");
        plot.YLabel("Average time, ms");
        plot.SavePng(path, 900, 600);
    }
}
