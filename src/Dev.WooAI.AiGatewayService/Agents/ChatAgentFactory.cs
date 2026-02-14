using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Services.Common.Contracts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Dev.WooAI.AiGatewayService.Agents;

public class ChatAgentFactory(IServiceProvider serviceProvider)
{

    private async Task<(LanguageModel, ConversationTemplate)> GetModelAndTemplateAsync(
       Expression<Func<ConversationTemplate, bool>> predicate)
    {
        using var scope = serviceProvider.CreateScope();
        var data = scope.ServiceProvider.GetRequiredService<IDataQueryService>();
        var query =
            from template in data.ConversationTemplates.Where(predicate)
             join model in data.LanguageModels on template.ModelId equals model.Id
            select new { model, template };

        var result = await data.FirstOrDefaultAsync(query);
        if (result == null) throw new Exception("未找对话模板或模型");
        return (result.model, result.template);
    }
    public ChatClientAgent CreateAgentAsync(LanguageModel model, ConversationTemplate template, SessionSoreState? sessionSoreState)
    {
        using var scope = serviceProvider.CreateScope();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("OpenAI");

        var chatClientBuilder = new OpenAIClient(
                new ApiKeyCredential(model.ApiKey ?? string.Empty),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(model.BaseUrl),
                    Transport = new HttpClientPipelineTransport(httpClient)
                })
            .GetChatClient(model.Name)
            .AsIChatClient()
            .AsBuilder()
            .UseOpenTelemetry(sourceName: nameof(AiGatewayService), configure: cfg => cfg.EnableSensitiveData = true);

        var chatOptions = new ChatOptions
        {
            Temperature = template.Specification.Temperature ?? model.Parameters.Temperature,
            Instructions = template.SystemPrompt
        };
        bool isSession = false;
        JsonElement json = new() ;
        if (sessionSoreState is not null)
        {
            json = JsonSerializer.SerializeToElement(sessionSoreState);
            isSession = true;
        }
        
        var agent = chatClientBuilder.BuildAIAgent(new ChatClientAgentOptions
        {
            Name = template.Name,
            ChatOptions = chatOptions,
            ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
                    new WooPgChatHistoryProvider(
                        serviceProvider,
                        (isSession ? json : ctx.SerializedState),
                        ctx.JsonSerializerOptions
                        )
                    )
        });

        return agent;

    }

    public async Task<ChatClientAgent> CreateAgentAsync(Guid templateId, SessionSoreState? sessionSoreState=null)
    {
        var (model, template) = await GetModelAndTemplateAsync(t => t.Id == templateId);
        return CreateAgentAsync(model, template, sessionSoreState);
    }

    public async Task<ChatClientAgent> CreateAgentAsync(string templateName,
        Action<ConversationTemplate>? configureTemplate = null, SessionSoreState? sessionSoreState = null)
    {
        var (model, template) = await GetModelAndTemplateAsync(t => t.Name == templateName);
        configureTemplate?.Invoke(template);
        return CreateAgentAsync(model, template, sessionSoreState);
    }

    //public async Task<ChatClientAgent> CreateAgentAsync(Guid templateId,Guid sessionId)
    //{
    //    var (model, template) = await GetModelAndTemplateAsync(t => t.Id == templateId);

    //    var httpClient = httpClientFactory.CreateClient("OpenAI");
    //    var json = JsonSerializer.SerializeToElement(sessionId);
    //    var agent = new OpenAIClient(
    //            new ApiKeyCredential(result.Model.ApiKey),
    //            new OpenAIClientOptions
    //            {
    //                Endpoint = new Uri(result.Model.BaseUrl),
    //                // 接管 OpenAI 的底层传输
    //                Transport = new HttpClientPipelineTransport(httpClient)
    //            })
    //        .GetChatClient(result.Model.Name)
    //        .AsIChatClient()
    //        .AsBuilder()
    //        .UseOpenTelemetry(sourceName: nameof(AiGatewayService), configure: client => client.EnableSensitiveData = true)
    //        .BuildAIAgent(new ChatClientAgentOptions
    //        {
    //            Name = result.Template.Name,
    //            ChatOptions = new ChatOptions
    //            {
    //                Temperature = result.Template.Temperature ?? result.Model.Temperature
    //            },
    //            ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
    //                new WooPgChatHistoryProvider(
    //                    serviceProvider,
    //                    json,
    //                    //ctx.SerializedState,
    //                    ctx.JsonSerializerOptions
    //                    )
    //                )

    //        });
    //    return agent;
    //}
}