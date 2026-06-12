using System;
using System.Threading.Tasks;
using LeanBusinessWorkflow.AI.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LeanBusinessWorkflow.AI.Services;

/// <summary>
/// AI Chat service powered by Semantic Kernel.
/// Provides conversational capabilities with optional memory/context from notes.
/// </summary>
public class ChatService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly ChatHistory _chatHistory;
    private readonly ILogger<ChatService> _logger;

    public ChatService(AiSettings settings, ILogger<ChatService> logger)
    {
        _logger = logger;
        _chatHistory = new ChatHistory();

        var builder = Kernel.CreateBuilder();

        if (settings.UseAzureOpenAI && !string.IsNullOrEmpty(settings.Endpoint))
        {
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: settings.DeploymentName ?? settings.ModelId,
                endpoint: settings.Endpoint,
                apiKey: settings.ApiKey ?? throw new ArgumentException("Azure OpenAI requires an API key"));
        }
        else
        {
            builder.AddOpenAIChatCompletion(
                modelId: settings.ModelId,
                apiKey: settings.ApiKey ?? throw new ArgumentException("OpenAI requires an API key"));
        }

        _kernel = builder.Build();
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();

        _chatHistory.AddSystemMessage("You are a helpful AI assistant for Lean Business Workflow. " +
            "You have access to an Obsidian-style knowledge base. Be concise and useful.");
    }

    /// <summary>
    /// Sends a message and gets a response from the AI.
    /// </summary>
    public async Task<string> SendMessageAsync(string message)
    {
        _chatHistory.AddUserMessage(message);

        try
        {
            var response = await _chatService.GetChatMessageContentAsync(_chatHistory);
            _chatHistory.AddMessage(response.Role, response.Content ?? "");

            return response.Content ?? "No response generated.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Semantic Kernel");
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Clears the conversation history.
    /// </summary>
    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage("You are a helpful AI assistant for Lean Business Workflow.");
    }

    /// <summary>
    /// Gets the current chat history.
    /// </summary>
    public ChatHistory GetHistory() => _chatHistory;
}