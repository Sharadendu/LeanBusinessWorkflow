using LeanBusinessWorkflow.AI.Configuration;
using LeanBusinessWorkflow.AI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LeanBusinessWorkflow.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddLeanBusinessWorkflowAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection("AI"));

        services.AddSingleton<AiSettings>(sp =>
        {
            var settings = new AiSettings();
            configuration.GetSection("AI").Bind(settings);
            return settings;
        });

        services.AddSingleton<ChatService>(sp =>
        {
            var settings = sp.GetRequiredService<AiSettings>();
            var logger = sp.GetRequiredService<ILogger<ChatService>>();
            return new ChatService(settings, logger);
        });

        return services;
    }
}