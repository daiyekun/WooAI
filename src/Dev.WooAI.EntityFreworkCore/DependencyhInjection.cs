using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.WooAI.EntityFreworkCore;

public static class DependencyhInjection
{
    public static IServiceCollection AddInfrastructres(this IServiceCollection services,IConfiguration configuration)
    {
        ConfigureIdentity(services);
        return services;
    }

    
    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WooAiDbContext>();
    }
}
