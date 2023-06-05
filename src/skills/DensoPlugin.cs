using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

public class DensoPlugin
{
    [SKFunction("Run the work command for the robot")]
    public string Work()
    {
        return "WORK";
    }

    [SKFunction("Run the dance command for the robot")]
    public string Dance()
    {
        return "DANCE";
    }

    [SKFunction("Get robot speed")]
    public string Speed()
    {
        return "25%";
    }
}