using Dev.WooAI.AgentPlugin;
using Dev.WooAI.AiGatewayService.Agents;
using Dev.WooAI.AiGatewayService.Plugins;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Dev.WooAI.AiGatewayService.Sessions.Commands;
#region  不需要
//[AuthorizeRequirement("AiGateway.SendUserMessage")]
//public record SendUserMessageCommand(Guid SessionId, string Content) : ICommand<IAsyncEnumerable<string>>;

//public class SendUserMessageCommandHandler(IRepository<Session> repo, ChatAgentFactory chatAgent)
//    : ICommandHandler<SendUserMessageCommand, IAsyncEnumerable<string>>
//{
//    public async Task<IAsyncEnumerable<string>> Handle(SendUserMessageCommand request, CancellationToken cancellationToken)
//    {
//        var session = await repo.GetByIdAsync(request.SessionId, cancellationToken);
//        if (session == null) throw new Exception("未找到会话");

//        var agent = await chatAgent.CreateAgentAsync(session.TemplateId);
        
//        var storeState = new { storeState = request.SessionId }; // 这里创建了一个匿名对象
//        var agentThread = agent.DeserializeThread(JsonSerializer.SerializeToElement(storeState));
        
//        // 返回迭代器函数
//        return await Task.FromResult(GetStreamAsync(agent, agentThread, request.Content, cancellationToken));
//    }

//    private async IAsyncEnumerable<string> GetStreamAsync(
//        ChatClientAgent agent, AgentThread thread, string content, [EnumeratorCancellation] CancellationToken cancellationToken)
//    {
//        // 调用Agent流式读取响应
//        await foreach (var update in agent.RunStreamingAsync(content, thread, cancellationToken: cancellationToken))
//        {
//            yield return update.Text;
//        }
//    }
//}
#endregion

#region 现在需要
[AuthorizeRequirement("AiGateway.SendUserMessage")]
public record SendUserMessageCommand(Guid SessionId, string Content) : ICommand<IAsyncEnumerable<string>>;

public class SendUserMessageCommandHandler(
    IRepository<Session> repo, 
    ChatAgentFactory chatAgent,
    AgentPluginLoader pluginLoader)
    : ICommandHandler<SendUserMessageCommand, IAsyncEnumerable<string>>
{
    public async Task<IAsyncEnumerable<string>> Handle(SendUserMessageCommand request, CancellationToken cancellationToken)
    {
        var session = await repo.GetByIdAsync(request.SessionId, cancellationToken);
        if (session == null) throw new Exception("未找到会话");
        var sessionState = new SessionSoreState(request.SessionId);
        var agent = await chatAgent.CreateAgentAsync(session.TemplateId, sessionState);

        //todo:不能传入当前sessionId 去创建
        var curragentSession=await agent.CreateSessionAsync();
        //var storeState = new { storeState = request.SessionId }; // 这里创建了一个匿名对象
        //var agentThread = agent.DeserializeThread(JsonSerializer.SerializeToElement(storeState));
        //JsonElement serializedSession = agent.SerializeSession(curragentSession);

        // Later, deserialize the session to resume the conversation
        //AgentSession resumedSession = await agent.DeserializeSessionAsync(serializedSession);
        // 返回迭代器函数
        return await Task.FromResult(GetStreamAsync(agent, curragentSession, request.Content, cancellationToken));
    }

    private async IAsyncEnumerable<string> GetStreamAsync(
        ChatClientAgent agent, AgentSession agentSession, string inputcontent, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var tools = pluginLoader.GetAITools(nameof(TimePlugin));

        // 调用Agent流式读取响应
        await foreach (var update in agent.RunStreamingAsync(inputcontent, agentSession,
            new ChatClientAgentRunOptions { 
                ChatOptions=new ChatOptions
                {
                    Tools = tools
                }
               }, cancellationToken: cancellationToken))
        {
            foreach (var content in update.Contents)
            {
                switch (content)
                {
                    case TextContent callContent:
                        yield return callContent.Text;
                        break;
                    case FunctionCallContent callContent:
                        yield return $"\n\n```\n正在执行工具：{callContent.Name} \n请求参数：{JsonSerializer.Serialize(callContent.Arguments)}";
                        break;
                    case FunctionResultContent callContent:
                        yield return $"\n\n执行结果：{JsonSerializer.Serialize(callContent.Result)}\n```\n\n";
                        break;
                }
            }
        }
    }
}
#endregion