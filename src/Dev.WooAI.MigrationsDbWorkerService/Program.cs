using Dev.WooAI.EntityFreworkCore;
using Dev.WooAI.MigrationsDbWorkerService;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddInfrastructres(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<WooAiDbContext>("dev-wooai");

var host = builder.Build();
host.Run();
