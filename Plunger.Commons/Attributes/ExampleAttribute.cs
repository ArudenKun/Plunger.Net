namespace Plunger.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ExampleAttribute : Attribute
{
    public string Example { get; }

    public ExampleAttribute(string example)
    {
        Example = example;
    }
}
