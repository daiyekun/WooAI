using Dev.WooAI.Services.Common.Contracts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Dev.WooAI.AiGatewayService.Agents;

public class ChatAgentFactory(
    IDataQueryService data, 
    IServiceProvider serviceProvider,
    IHttpClientFactory httpClientFactory)
{
    public async Task<ChatClientAgent> CreateAgentAsync(Guid templateId)
    {
        var queryable =
            from template in data.ConversationTemplates
            join model in data.LanguageModels on template.ModelId equals model.Id
            where template.Id == templateId
            select new
            {
                Model = new
                {
                    model.BaseUrl,
                    model.ApiKey,
                    model.Name,
                    model.Parameters.Temperature
                },
                Template = new
                {
                    template.Name,
                    template.SystemPrompt,
                    template.Specification.Temperature
                }
            };

        var result = await data.FirstOrDefaultAsync(queryable);
        if (result == null) throw new Exception("未找对话模板或模型");
        
        var httpClient = httpClientFactory.CreateClient("OpenAI");

        var agent = new OpenAIClient(
                new ApiKeyCredential(result.Model.ApiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(result.Model.BaseUrl),
                    // 接管 OpenAI 的底层传输
                    Transport = new HttpClientPipelineTransport(httpClient)
                })
            .GetChatClient(result.Model.Name)
            .AsAIAgent(new ChatClientAgentOptions {
                Name = result.Template.Name,
                ChatOptions=new ChatOptions
                {
                    Temperature = result.Template.Temperature ?? result.Model.Temperature
                },
                ChatHistoryProviderFactory = (ctx, ct) =>new ValueTask<ChatHistoryProvider>(
                    new WooPgChatHistoryProvider(
                        serviceProvider,
                        ctx.SerializedState,
                        ctx.JsonSerializerOptions
                        )
                    )
              
            });
            //.CreateAIAgent(new ChatClientAgentOptions
            //{
            //    Name = result.Template.Name,
            //    Instructions = result.Template.SystemPrompt,
            //    ChatOptions = new ChatOptions
            //    {
            //        Temperature = result.Template.Temperature ?? result.Model.Temperature
            //    },
            //    ChatMessageStoreFactory = context => new SessionChatMessageStore(serviceProvider, context.SerializedState)
            //});

        return agent;
    }

    public async Task<ChatClientAgent> CreateAgentAsync(Guid templateId,Guid sessionId)
    {
        var queryable =
            from template in data.ConversationTemplates
            join model in data.LanguageModels on template.ModelId equals model.Id
            where template.Id == templateId
            select new
            {
                Model = new
                {
                    model.BaseUrl,
                    model.ApiKey,
                    model.Name,
                    model.Parameters.Temperature
                },
                Template = new
                {
                    template.Name,
                    template.SystemPrompt,
                    template.Specification.Temperature
                }
            };

        var result = await data.FirstOrDefaultAsync(queryable);
        if (result == null) throw new Exception("未找对话模板或模型");

        var httpClient = httpClientFactory.CreateClient("OpenAI");
        var json = JsonSerializer.SerializeToElement(sessionId);
        var agent = new OpenAIClient(
                new ApiKeyCredential(result.Model.ApiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(result.Model.BaseUrl),
                    // 接管 OpenAI 的底层传输
                    Transport = new HttpClientPipelineTransport(httpClient)
                })
            .GetChatClient(result.Model.Name)
            .AsAIAgent(new ChatClientAgentOptions
            {
                Name = result.Template.Name,
                ChatOptions = new ChatOptions
                {
                    Temperature = result.Template.Temperature ?? result.Model.Temperature
                },
                ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
                    new WooPgChatHistoryProvider(
                        serviceProvider,
                        json,
                        //ctx.SerializedState,
                        ctx.JsonSerializerOptions
                        )
                    )

            });
        //.CreateAIAgent(new ChatClientAgentOptions
        //{
        //    Name = result.Template.Name,
        //    Instructions = result.Template.SystemPrompt,
        //    ChatOptions = new ChatOptions
        //    {
        //        Temperature = result.Template.Temperature ?? result.Model.Temperature
        //    },
        //    ChatMessageStoreFactory = context => new SessionChatMessageStore(serviceProvider, context.SerializedState)
        //});

        return agent;
    }
}