using Dev.WooAI.EntityFreworkCore;
using Dev.WooAI.IdentityService.Contracts;
using Dev.WooAI.Infrastructure.Authentication;
using Dev.WooAI.Infrastructure.Storage;
using Dev.WooAI.Services.Common.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Dev.WooAI.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructures(this IHostApplicationBuilder builder)
    {
        builder.AddEfCore();
        builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}