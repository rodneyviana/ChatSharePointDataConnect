using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

// get the model ID, endpoint, and API key from the Azure OpenAI portal
Env.Load();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
string modelId = Environment.GetEnvironmentVariable("MODELID");
string endpoint = Environment.GetEnvironmentVariable("ENDPOINT");
string apiKey = Environment.GetEnvironmentVariable("APIKEY");
string connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

if (String.IsNullOrWhiteSpace(modelId) || String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrWhiteSpace(endpoint) || String.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Please set the MODELID, ENDPOINT, CONNECTIONSTRING and APIKEY environment variables.");
    return;
}

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// Add enterprise components
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Warning));

// Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin to run sql queries
kernel.Plugins.AddFromType<SqlQueryPlugin>("SqlQuery");


// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Create a history to store the conversation
var history = new ChatHistory();

// To change instructions, modify the tablesdefinition.txt file
// The instructions were based on the schema definition of Graph Data Connector, found here: https://github.com/microsoftgraph/dataconnect-solutions/tree/main/datasetschemas
string instructions = File.ReadAllText("tablesdefinition.txt");


history.AddSystemMessage(instructions);

// Initiate a back-and-forth chat
StringBuilder userInput = new StringBuilder();
do
{
    while (userInput.Length == 0)
    {
        // Collect user input
        Console.Write("User > ");
        userInput.Append(Console.ReadLine() ?? "");
        if (userInput.ToString().ToLower() == "q")
        {
            Console.WriteLine("Goodbye!");
            return;
        }

    }

    // Add user input
    history.AddUserMessage(userInput.ToString());
    userInput.Clear();
    // Get the response from the AI
    // If the error is Microsoft.SemanticKernel.HttpOperationException then wait for 5 seconds and try again
    try
    {
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);

        // Print the results
        Console.WriteLine("Assistant > " + result);

        // Add the message from the agent to the chat history
        history.AddMessage(result.Role, result.Content ?? string.Empty);
    }
    catch (Microsoft.SemanticKernel.HttpOperationException ex)
    {
        Console.WriteLine($"Error: {ex.ToString()}");
        System.Threading.Thread.Sleep(5000);
    }
} while (true);
