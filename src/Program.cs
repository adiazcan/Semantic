﻿using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.Memory;

var kernelSettings = KernelSettings.LoadSettings();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<IKernel>())
    .WithMemoryStorage(new VolatileMemoryStore())
    .WithAzureTextEmbeddingGenerationService(deploymentName: kernelSettings.EmbeddingModelId, endpoint: kernelSettings.Endpoint, apiKey: kernelSettings.ApiKey, serviceId: kernelSettings.ServiceId)
    .WithAzureChatCompletionService(deploymentName: kernelSettings.DeploymentOrModelId, endpoint: kernelSettings.Endpoint, apiKey: kernelSettings.ApiKey, serviceId: kernelSettings.ServiceId)
    .Build();


// 1. Skill from File
//await Skills.RunSkillFromFile(kernel);


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


