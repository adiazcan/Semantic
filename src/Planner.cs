using System.Diagnostics;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Sequential;

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

        // var originalPlanResult = await originalPlan.InvokeAsync();

        // Console.WriteLine("Original Plan results:\n");
        // Console.WriteLine(originalPlanResult.Result);

        var newPlanResult = await kernel.RunAsync(newPlan);
        Console.WriteLine("New Plan results:\n");
        Console.WriteLine(newPlanResult.Result);
    }

    public static async Task RunActionPlannerWithIntent(IKernel kernel, string ask)
    {
        var planner = new SequentialPlanner(kernel);
        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
        var densoPlugin = kernel.ImportSkill ( new DensoPlugin(), "DensoPlugin");
        
        var plan = await planner.CreatePlanAsync(ask);
        Console.WriteLine("Plan:\n");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        await ExecutePlanAsync(kernel, plan);
        // var result = await kernel.RunAsync(plan);
        // Console.WriteLine("Result:\n");
        // Console.WriteLine(result.Result);
    }

    public static async Task EmailSamplesAsync(IKernel kernel)
    {
        Console.WriteLine("======== Sequential Planner - Create Email Plan ========");
        kernel.ImportSkill(new EmailSkill(), "email");

        // Load additional skills to enable planner to do non-trivial asks.
        string folder = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");;
        kernel.ImportSemanticSkillFromDirectory(folder,
            "SummarizeSkill",
            "WriterSkill");
            
        var planner = new SequentialPlanner(kernel, new SequentialPlannerConfig());
        var plan = await planner.CreatePlanAsync("Summarize an input, translate to spanish, and e-mail to John Doe");

        // Original plan:
        // Goal: Summarize an input, translate to french, and e-mail to John Doe

        // Steps:
        // - SummarizeSkill.Summarize INPUT='' =>
        // - WriterSkill.Translate language='French' INPUT='' => TRANSLATED_SUMMARY
        // - email.GetEmailAddress INPUT='John Doe' => EMAIL_ADDRESS
        // - email.SendEmail INPUT='$TRANSLATED_SUMMARY' email_address='$EMAIL_ADDRESS' =>

        Console.WriteLine("Original plan:");
        Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        var input =
            "We are writing to introduce you to Gada-i, a product from ENCAMINA that helps you with intelligent document " +
            "archiving in Azure. Gada-i is an artificial intelligence service that automatically classifies and manages the " +
            "lifecycle of your documents, saving you storage costs and improving your document security. Gada-i can connect to " +
            "your existing document management system or corporate repository and allow you to access your archived documents " +
            "at any time through a web application. Gada-i uses Microsoft Azure technology and follows its security paradigm. " +
            "If you are interested in learning more about Gada-i and how it can benefit your business, please visit our website " +
            "or contact us for a free demo. We would love to hear from you and answer any questions you may have.";
        
        Console.WriteLine("======== Sequential Planner - Execute Email Plan ========");

        await ExecutePlanAsync(kernel, plan, input, 5);
    }    

    private static async Task<Plan> ExecutePlanAsync(IKernel kernel, Plan plan, string input = "", int maxSteps = 10)
    {
        Stopwatch sw = new();
        sw.Start();

        // loop until complete or at most N steps
        try
        {
            for (int step = 1; plan.HasNextStep && step < maxSteps; step++)
            {
                if (string.IsNullOrEmpty(input))
                {
                    await plan.InvokeNextStepAsync(kernel.CreateNewContext());
                    // or await kernel.StepAsync(plan);
                }
                else
                {
                    plan = await kernel.StepAsync(input, plan);
                }

                if (!plan.HasNextStep)
                {
                    PrintStep($"Step {step} {plan.Steps[step-1].Name} - COMPLETE!");
                    Console.WriteLine(plan.State.ToString());
                    break;
                }

                PrintStep($"Step {step} {plan.Steps[step-1].Name} - Results so far:");
                Console.WriteLine(plan.State.ToString());
            }
        }
        catch (KernelException e)
        {

            PrintStep("Step - Execution failed:");
            Console.WriteLine(e.Message);
        }

        sw.Stop();
        Console.WriteLine($"Execution complete in {sw.ElapsedMilliseconds} ms!");
        return plan;
    }

    private static void PrintStep(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}