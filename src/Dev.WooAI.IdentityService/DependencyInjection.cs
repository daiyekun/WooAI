using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.WooAI.IdentityService;

public static class DependencyInjection
{
    public static void AddIdentityService(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        { 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    }
}