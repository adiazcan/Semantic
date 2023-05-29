using Microsoft.SemanticKernel.SkillDefinition;

public class MyCSharpPlugin
{
    [SKFunction("Return the first row of a qwerty keyboard")]
    public string Qwerty(string input)
    {
        return "qwertyuiop";
    }

    [SKFunction("Return a string that's duplicated")]
    public string DupDup(string text)
    {
        return text + text;
    }
}