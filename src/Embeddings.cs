using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Skills.Core;

public class Embeddings
{
    const string MemoryCollectionName = "aboutMe";
    const string ghMemoryCollectionName = "SKGitHub";

    public static async Task AddEmbeddings(IKernel kernel)
    {

        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "My name is Andrea");
        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "I currently work as a tourist operator");
        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "I currently live in Seattle and have been living there since 2005");
        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");
        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info5", text: "My family is from New York");   
    }

    public static async Task SearchMemory(IKernel kernel)
    {
        var questions = new[]
        {
            "what is my name?",
            "where do I live?",
            "where is my family from?",
            "where have I travelled?",
            "what do I do for work?",
        };

        foreach (var q in questions)
        {
            // var response = kernel.Memory.SearchAsync(MemoryCollectionName, q);
            // Console.WriteLine(q + " " + await response ?.Metadata.Text);
        }        
    }

    public static async Task Recall(IKernel kernel)
    {
        kernel.ImportSkill(new TextMemorySkill());

        const string skPrompt = @"
            ChatBot can have a conversation with you about any topic.
            It can give explicit instructions or say 'I don't know' if it does not have an answer.

            Information about me, from previous conversations:
            - {{$fact1}} {{recall $fact1}}
            - {{$fact2}} {{recall $fact2}}
            - {{$fact3}} {{recall $fact3}}
            - {{$fact4}} {{recall $fact4}}
            - {{$fact5}} {{recall $fact5}}

            Chat:
            {{$history}}
            User: {{$userInput}}
            ChatBot: ";

        var chatFunction = kernel.CreateSemanticFunction(skPrompt, maxTokens: 200, temperature: 0.8);

        var context = kernel.CreateNewContext();

        context["fact1"] = "what is my name?";
        context["fact2"] = "where do I live?";
        context["fact3"] = "where is my family from?";
        context["fact4"] = "where have I travelled?";
        context["fact5"] = "what do I do for work?";

        context[TextMemorySkill.CollectionParam] = MemoryCollectionName;
        context[TextMemorySkill.RelevanceParam] = "0.8";      

        var history = "";
        context["history"] = history;
        Func<string, Task> Chat = async (string input) => {
            // Save new message in the context variables
            context["userInput"] = input;

            // Process the user message and get an answer
            var answer = await chatFunction.InvokeAsync(context);

            // Append the new interaction to the chat history
            history += $"\nUser: {input}\nChatBot: {answer}\n";
            context["history"] = history;
            
            // Show the bot response
            Console.WriteLine("ChatBot: " + context);
        };

        await Chat("Hello, I think we've met before, remember? my name is...");
        await Chat("I want to plan a trip and visit my family. Do you know where that is?");
        await Chat("Great! What are some fun things to do there?");
    }

    public static async Task AddDocumentToMemory(IKernel kernel)
    {
        var githubFiles = new Dictionary<string, string>()
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
                = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/Volatile/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample starter react-based chat summary webapp",
        };

        Console.WriteLine("Adding some GitHub file URLs and their descriptions to a volatile Semantic Memory.");
        var i = 0;
        foreach (var entry in githubFiles)
        {
            await kernel.Memory.SaveReferenceAsync(
                collection: ghMemoryCollectionName,
                description: entry.Value,
                text: entry.Value,
                externalId: entry.Key,
                externalSourceName: "GitHub"
            );
            Console.WriteLine($"  URL {++i} saved");
        }
    }

    public static async Task SearchGitHub(IKernel kernel)
    {
        string ask = "I love Jupyter notebooks, how should I get started?";
        Console.WriteLine("===========================\n" +
                            "Query: " + ask + "\n");

        var memories = kernel.Memory.SearchAsync(ghMemoryCollectionName, ask, limit: 5, minRelevanceScore: 0.77);

        var i = 0;
        await foreach (var memory in memories)
        {
            Console.WriteLine($"Result {++i}:");
            Console.WriteLine("  URL:     : " + memory.Metadata.Id);
            Console.WriteLine("  Title    : " + memory.Metadata.Description);
            Console.WriteLine("  Relevance: " + memory.Relevance);
            Console.WriteLine();
        }
    }
}