using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dev.WooAI.AiGatewayService.Agents;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;


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

public class SendUserMessageCommandHandler(IRepository<Session> repo, ChatAgentFactory chatAgent)
    : ICommandHandler<SendUserMessageCommand, IAsyncEnumerable<string>>
{
    public async Task<IAsyncEnumerable<string>> Handle(SendUserMessageCommand request, CancellationToken cancellationToken)
    {
        var session = await repo.GetByIdAsync(request.SessionId, cancellationToken);
        if (session == null) throw new Exception("未找到会话");

        var agent = await chatAgent.CreateAgentAsync(session.TemplateId, request.SessionId);

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
        ChatClientAgent agent, AgentSession agentSession, string content, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // 调用Agent流式读取响应
        await foreach (var update in agent.RunStreamingAsync(content, agentSession, cancellationToken: cancellationToken))
        {
            yield return update.Text;
        }
    }
}
#endregion