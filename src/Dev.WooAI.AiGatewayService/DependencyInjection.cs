using Dev.WooAI.AgentPlugin;
using Dev.WooAI.AiGatewayService.Agents;
using Dev.WooAI.AiGatewayService.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Dev.WooAI.AiGatewayService;

public static class DependencyInjection
{
    public static void AddAiGatewayService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        builder.Services.AddSingleton<ChatAgentFactory>();

        builder.Services.AddHttpClient("OpenAI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddAgentPlugin(registrar =>
        {
            registrar.RegisterPluginFromAssembly(Assembly.GetExecutingAssembly());
        });

        builder.Services.AddSingleton<IntentRoutingAgentBuilder>();

        builder.AddIntentWorkflow();
    }
}