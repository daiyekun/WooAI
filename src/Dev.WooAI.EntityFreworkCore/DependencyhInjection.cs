using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Dev.WooAI.EntityFrameworkCore;
using Dev.WooAI.EntityFrameworkCore.Repository;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.Services.Common.Contracts;



namespace Dev.WooAI.EntityFreworkCore;

public static class DependencyhInjection
{
    public static void AddEfCore(this IHostApplicationBuilder builder)
    {
       
        builder.AddNpgsqlDbContext<WooAiDbContext>("dev-wooai");

        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>));
        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped<IDataQueryService, DataQueryService>();

        builder.Services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WooAiDbContext>();
    }
}
