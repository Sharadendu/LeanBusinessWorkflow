namespace LeanBusinessWorkflow.AI.Configuration;

public class AiSettings
{
    public string ModelId { get; set; } = "gpt-4o-mini";
    public string? ApiKey { get; set; }
    public string? Endpoint { get; set; } // For Azure OpenAI
    public string? DeploymentName { get; set; }
    public bool UseAzureOpenAI { get; set; } = false;
}