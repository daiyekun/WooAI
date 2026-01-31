using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;


namespace Dev.WooAI.EntityFreworkCore;

public static class DependencyhInjection
{
    public static void AddEfCore(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<WooAiDbContext>("dev-wooai");

        builder.Services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WooAiDbContext>();
    }
}
