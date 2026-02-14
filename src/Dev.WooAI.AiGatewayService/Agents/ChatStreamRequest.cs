using Dev.WooAI.AiGatewayService.Workflows;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Common.Contracts;
using MediatR;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;


namespace Dev.WooAI.AiGatewayService.Agents;

#region 老的写法
//[AuthorizeRequirement("AiGateway.Chat")]
//public record ChatStreamRequest(Guid SessionId, string Message) : IStreamRequest<object>;

//public class ChatStreamHandler(
//    IDataQueryService queryService, 
//    IntentRoutingAgentBuilder agentBuilder) : IStreamRequestHandler<ChatStreamRequest, object>
//{
//    public async IAsyncEnumerable<object> Handle(ChatStreamRequest request, CancellationToken cancellationToken)
//    {
//        if (!queryService.Sessions.Any(session => session.Id == request.SessionId))
//        {
//            throw new Exception("未找到会话");
//        }

//        var agent = await agentBuilder.BuildAsync();

//        await foreach (var update in agent.RunStreamingAsync(request.Message,cancellationToken:cancellationToken))
//        {
//            foreach (var content in update.Contents)
//            {
//                switch (content)
//                {
//                    case TextContent callContent:
//                        yield return new { content = callContent.Text };
//                        break;
//                    case FunctionCallContent callContent:
//                        yield return new
//                        {
//                            content =
//                                $"\n\n```\n正在执行工具：{callContent.Name} \n请求参数：{JsonSerializer.Serialize(callContent.Arguments)}"
//                        };
//                        break;
//                    case FunctionResultContent callContent:
//                        yield return new
//                        { content = $"\n\n执行结果：{JsonSerializer.Serialize(callContent.Result)}\n```\n\n" };
//                        break;
//                }
//            }
//        }



//    }
//}

#endregion

[AuthorizeRequirement("AiGateway.Chat")]
public record ChatStreamRequest(Guid SessionId, string Message) : IStreamRequest<object>;

public class ChatStreamHandler(
    IDataQueryService queryService,
    ILogger<ChatStreamHandler> logger,
    [FromKeyedServices(nameof(IntentWorkflow))] Workflow workflow)
    : IStreamRequestHandler<ChatStreamRequest, object>
{
    public async IAsyncEnumerable<object> Handle(ChatStreamRequest request, [EnumeratorCancellation]CancellationToken cancellationToken=default)
    {
        if (!queryService.Sessions.Any(session => session.Id == request.SessionId))
        {
            throw new Exception("未找到会话");
        }

        await using var run = await InProcessExecution.StreamAsync(workflow, request, cancellationToken: cancellationToken);
        await foreach (var workflowEvent in run.WatchStreamAsync(cancellationToken))
        {
            logger.LogInformation("curr Event：{evernttype}",workflowEvent.GetType());
            switch (workflowEvent)
            {
                case ExecutorFailedEvent evt:
                    yield return new { content = $"发生错误：{evt.Data.Message}" };
                    break;
                case AgentResponseEvent evt:
                    yield return new
                    {
                        content =
                            $"\n\n\n```json\n //意图分类\n {evt.Response.Text}\n```\n\n"
                    };
                    break;
                case AgentResponseUpdateEvent evt:
                    foreach (var content in evt.Update.Contents)
                    {
                        logger.LogInformation("curr content：{content} {json}", content.GetType(), JsonSerializer.Serialize(content));
                        switch (content)
                        {
                            case TextContent callContent:
                                yield return new { content = callContent.Text };
                                break;
                            case FunctionCallContent callContent:
                                yield return new
                                {
                                    content =
                                        $"\n\n```\n正在执行工具：{callContent.Name} \n请求参数：{JsonSerializer.Serialize(callContent.Arguments)}"
                                };
                                break;
                            case FunctionResultContent callContent:
                                yield return new
                                { content = $"\n\n执行结果：{JsonSerializer.Serialize(callContent.Result)}\n```\n\n" };
                                break;
                        }
                    }
                    break;
            }
        }
    }
}