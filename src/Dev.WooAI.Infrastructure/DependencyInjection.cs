using Dev.WooAI.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dev.WooAI.EntityFreworkCore;
using Dev.WooAI.IdentityService.Contracts;


namespace Dev.WooAI.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructures(this IHostApplicationBuilder builder)
    {
        builder.AddEfCore();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}