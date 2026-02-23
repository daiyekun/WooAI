using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dev.WooAI.EventBus;

namespace Dev.WooAI.RagService;

public static class DependencyInjection
{
    public static void AddRagService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        builder.AddEventBus();
    }
}