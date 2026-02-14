using Dev.WooAI.AiGatewayService.Agents;
using Dev.WooAI.Services.Common.Contracts;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Dev.WooAI.AiGatewayService.Workflows;

#region 老的写法
//public class FinalProcessExecutor(ChatAgentFactory agentFactory, IServiceProvider serviceProvider):
//    ReflectingExecutor<FinalProcessExecutor>("FinalProcessExecutor"),
//    IMessageHandler<AITool[]>
//{
//    public async ValueTask HandleAsync(AITool[] aiTools, IWorkflowContext context,
//        CancellationToken cancellationToken = new())
//    {
//        try
//        {
//            var request = await context.ReadStateAsync<ChatStreamRequest>("ChatStreamRequest", "Chat", cancellationToken: cancellationToken);
//            if (request == null) return;

//            var scope = serviceProvider.CreateScope();
//            var queryService = scope.ServiceProvider.GetRequiredService<IDataQueryService>();
//            var queryable = queryService.Sessions
//                .Where(s => s.Id == request.SessionId)
//                .Select(s => s.TemplateId);

//            var templateId = queryable.FirstOrDefault();

//            var agent = await agentFactory.CreateAgentAsync(templateId);
//            var storeThread = new { storeState = new SessionSoreState(request.SessionId) };
//            var agentThread = agent.DeserializeThread(JsonSerializer.SerializeToElement(storeThread));

//            await foreach (var update in agent.RunStreamingAsync(request.Message, agentThread, new ChatClientAgentRunOptions
//                           {
//                               ChatOptions = new ChatOptions
//                               {
//                                   Tools = aiTools
//                               }
//                           }, cancellationToken))
//            {
//                await context.AddEventAsync(new AgentResponseUpdateEvent(Id, update), cancellationToken);
//            }

//        }
//        catch (Exception e)
//        {
//            await context.AddEventAsync(new ExecutorFailedEvent(Id,e), cancellationToken);
//            throw;
//        }
//    }
//}

#endregion

public class FinalProcessExecutor(ChatAgentFactory agentFactory, IServiceProvider serviceProvider) : Executor(nameof(FinalProcessExecutor))
{
    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    {
        return routeBuilder.AddHandler<AITool[]>(HandleAsync);
    }

    [MessageHandler]
    public async ValueTask HandleAsync(AITool[] aiTools, IWorkflowContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var request = await context.ReadStateAsync<ChatStreamRequest>("ChatStreamRequest", "Chat", cancellationToken: cancellationToken);
            if (request == null) return;

            var scope = serviceProvider.CreateScope();
            var queryService = scope.ServiceProvider.GetRequiredService<IDataQueryService>();
            var queryable = queryService.Sessions
                .Where(s => s.Id == request.SessionId)
                .Select(s => s.TemplateId);

            var templateId = queryable.FirstOrDefault();
            var sessionState = new SessionSoreState(request.SessionId);
            var agent = await agentFactory.CreateAgentAsync(templateId, sessionState);
            //var storeThread = new { storeState = new SessionSoreState(request.SessionId) };
          
            //var agentThread = agent.DeserializeThread(JsonSerializer.SerializeToElement(storeThread));
            //var agentSession = new AgentSession().GetService<SessionSoreState>(sessionState);
            var session = await agent.CreateSessionAsync();
            await foreach (var update in agent.RunStreamingAsync(request.Message, session, new ChatClientAgentRunOptions
            {
                ChatOptions = new ChatOptions
                {
                    Tools = aiTools
                }
            }, cancellationToken))
            {
                await context.AddEventAsync(new AgentResponseUpdateEvent(Id, update), cancellationToken);
            }

        }
        catch (Exception e)
        {
            await context.AddEventAsync(new ExecutorFailedEvent(Id, e), cancellationToken);
            throw;
        }
    }

   
}