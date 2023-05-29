using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.SemanticFunctions;

public class Chat
{
    public static async Task RunChat(IKernel kernel)
    {
        const string skPrompt = @"
            ChatBot can have a conversation with you about any topic.
            It can give explicit instructions or say 'I don't know' if it does not have an answer.

            {{$history}}
            User: {{$userInput}}
            ChatBot:";

        var promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5,
            }
        };

        var promptTemplate = new PromptTemplate(skPrompt, promptConfig, kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var chatFunction = kernel.RegisterSemanticFunction("ChatBot", "Chat", functionConfig);

        var context = kernel.CreateNewContext();

        var history = "";
        context["history"] = history;

        var userInput = "Hi, I'm looking for book suggestions";
        context["userInput"] = userInput;

        var bot_answer = await chatFunction.InvokeAsync(context);

        history += $"\nUser: {userInput}\nMelody: {bot_answer}\n";
        context.Variables.Update(history);

        Console.WriteLine(context);

        Func<string, Task> Chat = async (string input) =>
        {
            // Save new message in the context variables
            context["userInput"] = input;

            // Process the user message and get an answer
            var answer = await chatFunction.InvokeAsync(context);

            // Append the new interaction to the chat history
            history += $"\nUser: {input}\nMelody: {answer}\n";
            context["history"] = history;

            // Show the response
            Console.WriteLine(context);
        };

        await Chat("I would like a non-fiction book suggestion about Greece history. Please only list one book.");
        await Chat("that sounds interesting, what are some of the topics I will learn about?");
        await Chat("Which topic from the ones you listed do you think most people find interesting?");
        await Chat("could you list some more books I could read about the topic(s) you mentioned?");
    }

    public static async Task RunChatGPT(IKernel kernel)
    {
        var systemMessage = @"ChatBot can have a conversation with you about any topic.
            It can give explicit instructions or say 'I don't know' if it does not have an answer.";

        var chatGPT = kernel.GetService<IChatCompletion>();
        var chat = (OpenAIChatHistory)chatGPT.CreateNewChat(systemMessage);

        await SendChatMessage("Hi, I'm looking for book suggestions", chatGPT, chat);
        await SendChatMessage("I would like a non-fiction book suggestion about Greece history. Please only list one book.", chatGPT, chat);
        await SendChatMessage("that sounds interesting, what are some of the topics I will learn about?", chatGPT, chat);
        await SendChatMessage("Which topic from the ones you listed do you think most people find interesting?", chatGPT, chat);
        await SendChatMessage("could you list some more books I could read about the topic(s) you mentioned?", chatGPT, chat);
    }

    private static async Task SendChatMessage(string userMessage, IChatCompletion chatGPT, OpenAIChatHistory chat)
    {
        Console.WriteLine($"User: {userMessage}");
        chat.AddUserMessage(userMessage);
        var reply = await chatGPT.GenerateMessageAsync(chat, new ChatRequestSettings());
        chat.AddAssistantMessage(reply);
        Console.WriteLine($"Assistant: {reply}");
    }
}