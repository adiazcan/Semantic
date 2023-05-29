using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

public class Planner
{
    public static async Task RunSequentialPlanner(IKernel kernel)
    {
        // Load native skill into the kernel registry, sharing its functions with prompt templates
        var planner = new SequentialPlanner(kernel);

        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
        kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "SummarizeSkill");
        kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "WriterSkill");

        var ask = "Tomorrow is Valentine's day. I need to come up with a few date ideas and e-mail them to my significant other.";
        var originalPlan = await planner.CreatePlanAsync(ask);

        Console.WriteLine("Original plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(originalPlan, new JsonSerializerOptions { WriteIndented = true }));

        string skPrompt = @"
            {{$input}}

            Rewrite the above in the style of Shakespeare.";
        var shakespeareFunction = kernel.CreateSemanticFunction(skPrompt, "shakespeare", "ShakespeareSkill", maxTokens: 2000, temperature: 0.2, topP: 0.5);

        var ask2 = @"Tomorrow is Valentine's day. I need to come up with a few date ideas.
            She likes Shakespeare so write using his style. E-mail these ideas to my significant other";

        var newPlan = await planner.CreatePlanAsync(ask2);

        Console.WriteLine("Updated plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(newPlan, new JsonSerializerOptions { WriteIndented = true }));

        var originalPlanResult = await originalPlan.InvokeAsync();

        Console.WriteLine("Original Plan results:\n");
        Console.WriteLine(originalPlanResult.Result);

        var newPlanResult = await kernel.RunAsync(newPlan);
        Console.WriteLine("New Plan results:\n");
        Console.WriteLine(newPlanResult.Result);
    }
}