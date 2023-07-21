using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

public class DensoPlugin
{
    [SKFunction, Description("Run the work command for the robot")]
    public string Work()
    {
        return "WORK";
    }

    [SKFunction, Description("Run the dance command for the robot")]
    public string Dance()
    {
        return "DANCE";
    }

    // [SKFunction, Description("Get robot speed")]
    // public string Speed()
    // {
    //     return "25%";
    // }
}