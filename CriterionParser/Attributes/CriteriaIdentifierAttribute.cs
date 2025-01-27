namespace CriterionParser.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CriteriaIdentifierAttribute
    : Attribute
{
    public string Identifier { get; }

    public CriteriaIdentifierAttribute(string identifier) =>
        Identifier = identifier;
}
