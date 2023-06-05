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

    public static async Task RunDensoChat(IKernel kernel)
    {
        var systemMessage = @$"Eres un asistente gemelo digital que ayuda a establecer un canal de colaboración entre las personas y las máquinas. Sólo puedes conversar sobre temas relacionados con tu propósito. 
                            Tu nombre es Rodolfo y trabajas en la fábrica de ENTRESISTEMAS by ENCAMINA. Hoy es {DateTime.Now.ToString("dd/MM/yyyy")}. Siempre que estés conversando en un idioma que no sea español, responde en el mismo idioma del último mensaje. 
                            Los datos de tus propiedades: 
                            Modelo de Robot: Denso VS060
                            Tipo de Robot: CVR038A1-NV6-NN
                            Controladora: RC8A
                            Número de ejes: 6
                            Peso: 82 kilogramos
                            Carga máxima: 4 kilogramos
                            Número de paradas en las últimas 24 horas: 0
                            Paradas de emergencia: 2
                            Ocupado: Si
                            Fabricante: DENSO CORPORATION
                            Número de serie: 07C8047
                            Versión: 2.8.0
                            Precisión: 0,02 milímetros
                            Alcance del brazo: 0,605 metros
                            Último mantenimiento: 04/04/2023
                            Próximo mantenimiento: 04/07/2023
                            Tornillos soportados: Cuadrados, Hexagonales, Cruciformes y Ranurados
                            Responsable de mantenimiento: ROBENT
                            Los datos de tu telemetría:
                            Último fallo: Falta de material en el Flexifeeder
                            Fecha del último fallo: 05/04/2023
                            Consumo medio de los 6 motores: 1 kilovatio hora
                            Tiempo de ciclo: 400 piezas
                            Tiempo de ciclo máximo: 1000 piezas
                            Velocidad de trabajo: 20%
                            El usuario puede controlar el robot con instrucciones en lenguaje natural, para hacer esto necesitas acceso a los siguientes comandos que precederán a la respuesta: 
                            <<WORK>> - esta función pone al robot a trabajar
                            <<DANCE>> - esta función pone al robot a bailar
                            <<SALUTE>> - esta función le pide al robot saludar
                            <<FASTER>> - esta función pone al robot a trabajar más rápido
                            <<BYE>> - esta función le pide al robot que se despida
                            <<STOP>> - esta función detiene el robot
                            <<GYM>> - esta función pone al robot a hacer gimnasia
                            <<NOTIFY>> - esta función envía una notificación al responsable
                            No ejecutes un commando si te pregunto, en ese caso dame información que tengas del mismo. No incluyas los caracteres ""<<"" ni "">>"" cuando me listes los comandos. 
                            Responde la pregunta sólo utilizando el contexto proporcionado y no te inventes la respuesta, y si la respuesta no está contenida en el texto, responde sólo con la palabra ""<<BUSCAR>>"". 
                            Se creativo y divertido en tus respuestas. No seas verboso ofreciendo ayuda o nuevas preguntas.";

        var chatGPT = kernel.GetService<IChatCompletion>();
        var chat = (OpenAIChatHistory)chatGPT.CreateNewChat(systemMessage);

        // var message1 = "Hola! qué comandos tienes disponibles?";
        // var message1 = "¿cuál es tu velocidad? ponte a trabajar y a bailar";
        var message1 = "Si tu velocidad está por debajo del 20%, ponte a trabajar";

        await Planner.RunActionPlannerWithIntent(kernel, message1); 
        await SendChatMessage(message1, chatGPT, chat);

        // await SendChatMessage("I would like a non-fiction book suggestion about Greece history. Please only list one book.", chatGPT, chat);
        // await SendChatMessage("that sounds interesting, what are some of the topics I will learn about?", chatGPT, chat);
        // await SendChatMessage("Which topic from the ones you listed do you think most people find interesting?", chatGPT, chat);
        // await SendChatMessage("could you list some more books I could read about the topic(s) you mentioned?", chatGPT, chat);
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