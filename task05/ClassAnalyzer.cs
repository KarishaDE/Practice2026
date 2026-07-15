using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
        => _type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(method => method.Name);

    public IEnumerable<string> GetMethodParams(string methodName)
        => _type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => method.Name == methodName)
            .SelectMany(method => method
                .GetParameters()
                .Select(parameter => parameter.Name ?? string.Empty)
                .Append(method.ReturnType.Name));

    public IEnumerable<string> GetAllFields()
        => _type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                       BindingFlags.Instance | BindingFlags.Static |
                       BindingFlags.DeclaredOnly)
            .Select(field => field.Name);

    public IEnumerable<string> GetProperties()
        => _type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance |
                           BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(property => property.Name);

    public bool HasAttribute<T>() where T : Attribute
        => _type.GetCustomAttribute<T>() is not null;
}
