using Microsoft.SemanticKernel;

public class Skills
{
    public static async Task RunSkillFromFile(IKernel kernel)
    {
        // note: using skills from the repo
        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
        var skill = kernel.ImportSemanticFunctionsFromDirectory(skillsDirectory, "FunSkill");

        var context = new ContextVariables();
        context.Set("input", "Time travel to dinosaur age");
        context.Set("style", "Wacky");

        var result = await kernel.RunAsync(context, skill["Joke"]);

        Console.WriteLine(result);
    }

    public static async Task RunSkillInline(IKernel kernel)
    {
        string skPrompt = @"
            {{$input}}

            Summarize the content above.";

        var promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0.2,
                TopP = 0.5,
            }
        };

        var promptTemplate = new PromptTemplate(
            skPrompt,
            promptConfig,
            kernel
        );

        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

        var summaryFunction = kernel.RegisterSemanticFunction("MySkill", "Summary", functionConfig);

        var input = @"
            Demo (ancient Greek poet)
            From Wikipedia, the free encyclopedia
            Demo or Damo (Greek: Δεμώ, Δαμώ; fl. c. AD 200) was a Greek woman of the Roman period, known for a single epigram, engraved upon the Colossus of Memnon, which bears her name. She speaks of herself therein as a lyric poetess dedicated to the Muses, but nothing is known of her life.[1]
            Identity
            Demo was evidently Greek, as her name, a traditional epithet of Demeter, signifies. The name was relatively common in the Hellenistic world, in Egypt and elsewhere, and she cannot be further identified. The date of her visit to the Colossus of Memnon cannot be established with certainty, but internal evidence on the left leg suggests her poem was inscribed there at some point in or after AD 196.[2]
            Epigram
            There are a number of graffiti inscriptions on the Colossus of Memnon. Following three epigrams by Julia Balbilla, a fourth epigram, in elegiac couplets, entitled and presumably authored by 'Demo' or 'Damo' (the Greek inscription is difficult to read), is a dedication to the Muses.[2] The poem is traditionally published with the works of Balbilla, though the internal evidence suggests a different author.[1]
            In the poem, Demo explains that Memnon has shown her special respect. In return, Demo offers the gift for poetry, as a gift to the hero. At the end of this epigram, she addresses Memnon, highlighting his divine status by recalling his strength and holiness.[2]
            Demo, like Julia Balbilla, writes in the artificial and poetic Aeolic dialect. The language indicates she was knowledgeable in Homeric poetry—'bearing a pleasant gift', for example, alludes to the use of that phrase throughout the Iliad and Odyssey.[a][2]";

        var summary = await summaryFunction.InvokeAsync(input);

        Console.WriteLine(summary);
    }

    public static async Task RunSkillInlineMin(IKernel kernel)
    {
        string skPrompt = @"
            {{$input}}

            Summarize the content above.";

        var textToSummarize = @"
            1) A robot may not injure a human being or, through inaction,
            allow a human being to come to harm.

            2) A robot must obey orders given it by human beings except where
            such orders would conflict with the First Law.

            3) A robot must protect its own existence as long as such protection
            does not conflict with the First or Second Law.
        ";

        var tldrFunction = kernel.CreateSemanticFunction(skPrompt, maxTokens: 200, temperature: 0, topP: 0.5);

        var summary2 = await tldrFunction.InvokeAsync(textToSummarize);

        Console.WriteLine(summary2);
    }

    public static async Task RunNativePlugin(IKernel kernel)
    {
        var myPlugin = kernel.ImportSkill(new MyCSharpPlugin(), "MyCSharpPlugin");
        var context = new ContextVariables();

        context.Set("INPUT","This is input.");

        var output = await kernel.RunAsync(context,myPlugin["DupDup"]);
        Console.WriteLine(output);
    }

    public static async Task RunSemanticWithNativePlugin(IKernel kernel)
    {
        var myContext = new ContextVariables("*Twinnify"); 
        var myCshPlugin = kernel.ImportSkill ( new MyCSharpPlugin(), "MyCSharpPlugin");
        var mySemPlugin = kernel.ImportSemanticSkillFromDirectory("skills", "MySemanticPlugin");
        var myOutput = await kernel.RunAsync(myContext,mySemPlugin["MySemanticFunction"]);

        Console.WriteLine(myOutput);
    }

    public static async Task RunNativePluginWithParams(IKernel kernel)
    {
        var myContext = new ContextVariables(); 
        myContext.Set("firstname","Sam");
        myContext.Set("lastname","Appdev");

        var myCshPlugin = kernel.ImportSkill ( new MyCSharpPlugin(), "MyCSharpPlugin");
        var myOutput = await kernel.RunAsync(myContext,myCshPlugin["FullNamer"]);

        Console.WriteLine(myOutput);
    }
}
