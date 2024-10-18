namespace Common;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class ConfigurationSectionAttribute : Attribute
{
    public ConfigurationSectionAttribute(string sectionName)
    {
        SectionName = sectionName;
    }

    public string SectionName { get; private set; }
}
