using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public static class DynamicCompiler
{
    public const string CalculatorSource = """
        using task11;

        public class Calculator : ICalculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }
        """;

    public static ICalculator CompileAndCreate()
    {
        return CompileAndCreate(CalculatorSource);
    }

    public static ICalculator CompileAndCreate(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var references = GetReferences();

        var compilation = CSharpCompilation.Create(
            $"DynamicCalculator_{Guid.NewGuid():N}",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic => diagnostic.GetMessage());

            throw new InvalidOperationException(
                $"Не удалось скомпилировать класс:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }

        var assembly = Assembly.Load(stream.ToArray());
        var calculatorType = assembly.GetType("Calculator")
            ?? throw new InvalidOperationException("Класс Calculator не найден");
        var calculator = Activator.CreateInstance(calculatorType) as ICalculator;

        return calculator
            ?? throw new InvalidOperationException("Класс Calculator не реализует ICalculator");
    }

    private static IEnumerable<MetadataReference> GetReferences()
    {
        var platformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string
            ?? throw new InvalidOperationException("Не найдены системные библиотеки");

        return platformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location));
    }
}
