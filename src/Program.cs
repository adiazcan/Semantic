using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Reliability;
using Microsoft.SemanticKernel.Reliability.Basic;

var kernelSettings = KernelSettings.LoadSettings();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

var memory = new MemoryBuilder()
    .WithLoggerFactory(loggerFactory)
    .WithAzureTextEmbeddingGenerationService(
        deploymentName: kernelSettings.EmbeddingModelId, 
        endpoint: kernelSettings.Endpoint, 
        apiKey: kernelSettings.ApiKey, 
        serviceId: kernelSettings.ServiceId
    )
    .WithMemoryStore(new VolatileMemoryStore())
    .Build();

var kernel = new KernelBuilder()
    .WithLoggerFactory(loggerFactory)
    .WithRetryBasic(
        new BasicRetryConfig { 
            MaxRetryCount = 3, 
            UseExponentialBackoff = true, 
            MinRetryDelay = TimeSpan.FromSeconds(3) 
        }
    )
    .WithAzureTextEmbeddingGenerationService(
        deploymentName: kernelSettings.EmbeddingModelId, 
        endpoint: kernelSettings.Endpoint, 
        apiKey: kernelSettings.ApiKey, 
        serviceId: kernelSettings.ServiceId
    )
    .WithAzureChatCompletionService(
        deploymentName: kernelSettings.DeploymentOrModelId, 
        endpoint: kernelSettings.Endpoint, 
        apiKey: kernelSettings.ApiKey, 
        serviceId: kernelSettings.ServiceId, 
        alsoAsTextCompletion: true,
        setAsDefault: true
    )
    .Build();


// 1. Skill from File
await Skills.RunSkillFromFile(kernel);


// Console.WriteLine("----------------------------------------");
// 2. Semantic Function Inline
//await Skills.RunSkillInline(kernel);

// Console.WriteLine("----------------------------------------");
// 3. Semantic Function Inline Min

// await Skills.RunSkillInlineMin(kernel);
// Console.WriteLine("----------------------------------------");
// 4. Chat

//await Chat.RunChat(kernel);

// Console.WriteLine("----------------------------------------");
// 5. Sequential Planner
//await Planner.RunSequentialPlanner(kernel);

// Console.WriteLine("----------------------------------------");

//6. Embeddings
// await Embeddings.AddEmbeddings(kernel);
// await Embeddings.SearchMemory(kernel);
// //await Embeddings.Recall(kernel);

// 7. Embeddings docs
// await Embeddings.AddDocumentToMemory(kernel);
// await Embeddings.SearchGitHub(kernel);
// Console.WriteLine("----------------------------------------");

// 8. Chat GPT
// await Chat.RunChatGPT(kernel);
// Console.WriteLine("----------------------------------------");

// 9. Native Plugin
// await Skills.RunNativePlugin(kernel);

// 10. Semantic Plugin with Native Plugin
// await Skills.RunSemanticWithNativePlugin(kernel);

// 11. Native Plugin with Params
// await Skills.RunNativePluginWithParams(kernel);

// await Chat.RunDensoChat(kernel);

//await Planner.EmailSamplesAsync(kernel);

// await Planner.RunDensoPlannerWithIntent(kernel, "cuál es tu velocidad y ponte a trabajar. ");
//await Planner.RunDensoPlannerWithIntent(kernel, "si tu velocidad está por debajo del 30%, ponte a trabajar. ");


//await Planner.EmailAllCustomersAsync(kernel);

// await Planner.BingStepwisePlanner(kernel, kernelSettings.BingApiKey);