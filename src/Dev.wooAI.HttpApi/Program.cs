using Dev.WooAI.EntityFreworkCore;
using Dev.WooAI.HttpApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<WooAiDbContext>("dev-wooai");
builder.Services.AddInfrastructres(builder.Configuration);
builder.Services.AddServiceUseCase();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(url: "/openapi/v1.json", name: "v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
