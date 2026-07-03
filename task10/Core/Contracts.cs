using System;

namespace Core;

public interface IExecutable
{
    void Run();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class HookAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AfterAttribute : Attribute
{
    public Type Target { get; }

    public AfterAttribute(Type target)
    {
        Target = target;
    }
}