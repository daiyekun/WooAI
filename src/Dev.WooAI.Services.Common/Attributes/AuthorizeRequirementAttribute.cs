namespace Dev.WooAI.Services.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeRequirementAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}