using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dev.WooAI.AgentPlugin;
using Dev.WooAI.AiGatewayService.Agents;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;


namespace Dev.WooAI.AiGatewayService.Workflows;

#region 老的写法
//public class ToolsPackExecutor(AgentPluginLoader pluginLoader):
//    ReflectingExecutor<ToolsPackExecutor>("ToolsPackExecutor"),
//    IMessageHandler<List<IntentResult>, AITool[]>
//{
//    public async ValueTask<AITool[]> HandleAsync(List<IntentResult> intentResults, IWorkflowContext context,
//        CancellationToken cancellationToken = new())
//    {
//        try
//        {
//            var intent = intentResults
//                .Where(i => i.Confidence >= 0.9)
//                .Select(i => i.Intent).ToArray();
//            var tools = pluginLoader.GetAITools(intent);

//            return tools;
//        }
//        catch (Exception e)
//        {
//            await context.AddEventAsync(new ExecutorFailedEvent(Id, e), cancellationToken);
//            throw;
//        }
//    }
//}
#endregion

public sealed partial class ToolsPackExecutor(AgentPluginLoader pluginLoader) : Executor(nameof(ToolsPackExecutor))
{
    protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
    {
        return routeBuilder.AddHandler<List<IntentResult>, AITool[]>(HandleAsync);
    }

    [MessageHandler]
    public async ValueTask<AITool[]> HandleAsync(List<IntentResult> intentResults, IWorkflowContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var intent = intentResults
                .Where(i => i.Confidence >= 0.75)
                .Select(i => i.Intent).ToArray();
            var tools = pluginLoader.GetAITools(intent);

            return tools;
        }
        catch (Exception e)
        {
            await context.AddEventAsync(new ExecutorFailedEvent(Id, e), cancellationToken);
            throw;
        }
    }
}