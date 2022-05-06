namespace Plunger.Commons.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class ValueAttribute : Attribute
{
    public ValueAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
