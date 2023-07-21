using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.SkillDefinition;

public class TextSkill
{
    [SKFunction, Description("Remove spaces to the left of a string")]
    [SKParameter("input", "Text to edit")]
    public string LStrip(string input)
    {
        return input.TrimStart();
    }

    [SKFunction, Description("Remove spaces to the right of a string")]
    [SKParameter("input", "Text to edit")]
    public string RStrip(string input)
    {
        return input.TrimEnd();
    }

    [SKFunction, Description("Remove spaces to the left and right of a string")]
    [SKParameter("input", "Text to edit")]
    public string Strip(string input)
    {
        return input.Trim();
    }

    [SKFunction, Description("Change all string chars to uppercase")]
    [SKParameter("input", "Text to uppercase")]
    public string Uppercase(string input)
    {
        return input.ToUpperInvariant();
    }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "By design.")]
    [SKFunction, Description("Change all string chars to lowercase")]
    [SKParameter("input", "Text to lowercase")]
    public string Lowercase(string input)
    {
        return input.ToLowerInvariant();
    }
}
