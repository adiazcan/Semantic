using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

var kernelSettings = KernelSettings.LoadSettings();

var kernelConfig = new KernelConfig();
kernelConfig.AddChatCompletionBackend(kernelSettings);
// kernelConfig.AddCompletionBackend(kernelSettings);

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder().WithLogger(loggerFactory.CreateLogger<IKernel>()).WithConfiguration(kernelConfig).Build();

// await Skills.RunSkillFromFile(kernel);
// Console.WriteLine("----------------------------------------");
// //Semantic Function Inline
// await Skills.RunSkillInline(kernel);
// Console.WriteLine("----------------------------------------");
// await Skills.RunSkillInlineMin(kernel);
// Console.WriteLine("----------------------------------------");
// await Chat.RunChat(kernel);

await Planner.RunSequentialPlanner(kernel);