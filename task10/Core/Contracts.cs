namespace Core;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PluginLoadAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PluginDependencyAttribute : Attribute
{
    public PluginDependencyAttribute(Type pluginType)
    {
        PluginType = pluginType;
    }

    public Type PluginType { get; }
}
