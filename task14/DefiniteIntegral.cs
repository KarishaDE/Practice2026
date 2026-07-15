using System.Collections.Concurrent;

namespace task14;

public static class DefiniteIntegral
{
    public static double SolveSingleThread(
        double a,
        double b,
        Func<double, double> function,
        double step)
    {
        ValidateArguments(a, b, function, step, 1);

        if (a == b)
        {
            return 0.0;
        }

        var sign = 1.0;
        if (a > b)
        {
            (a, b) = (b, a);
            sign = -1.0;
        }

        return sign * SolveSegment(a, b, function, step);
    }

    public static double Solve(
        double a,
        double b,
        Func<double, double> function,
        double step,
        int threadsNumber)
    {
        ValidateArguments(a, b, function, step, threadsNumber);

        if (a == b)
        {
            return 0.0;
        }

        var sign = 1.0;
        if (a > b)
        {
            (a, b) = (b, a);
            sign = -1.0;
        }

        var result = 0.0;
        var errors = new ConcurrentQueue<Exception>();
        var threads = new Thread[threadsNumber];
        var segmentLength = (b - a) / threadsNumber;

        using var barrier = new Barrier(threadsNumber + 1);

        for (var i = 0; i < threadsNumber; i++)
        {
            var segmentStart = a + segmentLength * i;
            var segmentEnd = i == threadsNumber - 1
                ? b
                : a + segmentLength * (i + 1);

            threads[i] = new Thread(() =>
            {
                try
                {
                    var segmentResult = SolveSegment(
                        segmentStart,
                        segmentEnd,
                        function,
                        step);

                    AddResult(ref result, segmentResult);
                }
                catch (Exception exception)
                {
                    errors.Enqueue(exception);
                }
                finally
                {
                    barrier.SignalAndWait();
                }
            });

            threads[i].IsBackground = true;
            threads[i].Start();
        }

        barrier.SignalAndWait();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        if (!errors.IsEmpty)
        {
            throw new AggregateException(errors);
        }

        return sign * result;
    }

    private static double SolveSegment(
        double start,
        double end,
        Func<double, double> function,
        double step)
    {
        var result = 0.0;
        var current = start;
        var currentValue = function(current);

        while (current < end)
        {
            var next = Math.Min(current + step, end);
            if (next <= current)
            {
                next = end;
            }

            var nextValue = function(next);
            result += (currentValue + nextValue) * (next - current) / 2.0;
            current = next;
            currentValue = nextValue;
        }

        return result;
    }

    private static void AddResult(ref double result, double value)
    {
        while (true)
        {
            var current = Volatile.Read(ref result);
            var updated = current + value;
            var observed = Interlocked.CompareExchange(ref result, updated, current);

            if (BitConverter.DoubleToInt64Bits(observed) ==
                BitConverter.DoubleToInt64Bits(current))
            {
                return;
            }
        }
    }

    private static void ValidateArguments(
        double a,
        double b,
        Func<double, double> function,
        double step,
        int threadsNumber)
    {
        ArgumentNullException.ThrowIfNull(function);

        if (!double.IsFinite(a) || !double.IsFinite(b))
        {
            throw new ArgumentOutOfRangeException(nameof(a), "Границы должны быть конечными числами");
        }

        if (!double.IsFinite(step) || step <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(step), "Шаг должен быть положительным числом");
        }

        if (threadsNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(threadsNumber),
                "Количество потоков должно быть положительным");
        }
    }
}
