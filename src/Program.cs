using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

var kernelSettings = KernelSettings.LoadSettings();

var kernelConfig = new KernelConfig();
kernelConfig.AddChatCompletionBackend(kernelSettings);
// kernelConfig.AddCompletionBackend(kernelSettings);
kernelConfig.AddEmbeddingsBackend(kernelSettings);

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<IKernel>())
    .WithConfiguration(kernelConfig)
    .WithMemoryStorage(new VolatileMemoryStore())
    .Build();

// await Skills.RunSkillFromFile(kernel);
// Console.WriteLine("----------------------------------------");
// //Semantic Function Inline
// await Skills.RunSkillInline(kernel);
// Console.WriteLine("----------------------------------------");
// await Skills.RunSkillInlineMin(kernel);
// Console.WriteLine("----------------------------------------");
// await Chat.RunChat(kernel);
// Console.WriteLine("----------------------------------------");
// await Planner.RunSequentialPlanner(kernel);
// Console.WriteLine("----------------------------------------");


// await Embeddings.AddEmbeddings(kernel);
// //await Embeddings.SearchMemory(kernel);
// //await Embeddings.Recall(kernel);
// await Embeddings.AddDocumentToMemory(kernel);
// await Embeddings.SearchGitHub(kernel);

await Chat.RunChatGPT(kernel);