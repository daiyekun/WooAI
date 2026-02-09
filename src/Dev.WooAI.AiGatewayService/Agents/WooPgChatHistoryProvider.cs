using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Services.Common.Contracts;
using Dev.WooAI.SharedKernel.Repository;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Dev.WooAI.AiGatewayService.Agents
{
    public sealed class WooPgChatHistoryProvider : ChatHistoryProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private Guid? SessionDbKey { get; set; }
        public WooPgChatHistoryProvider(
              IServiceProvider serviceProvider
            , JsonElement storeState
            ,JsonSerializerOptions? jsonSerializerOptions = null)
        {
            _serviceProvider = serviceProvider;
            if (storeState.ValueKind is JsonValueKind.String)
            {
                SessionDbKey = storeState.Deserialize<Guid>();
            }
        }

        public override async ValueTask InvokedAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
            if (context.InvokeException is not null)
            {
                return;
            }

            SessionDbKey ??= Guid.NewGuid();
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Session>>();

            // 加载聚合根
            var session = await repo.GetByIdAsync(SessionDbKey, cancellationToken);
            if (session == null) return;

            var allNewMessages = context.RequestMessages.Concat(context.AIContextProviderMessages ?? [])
                                .Concat(context.ResponseMessages ?? []);


            var hasNewMessage = false;
            foreach (var msg in allNewMessages)
            {
                // 将 Agent Role 转换为枚举
                // msg.Role.ToString() 可能返回 "user", "assistant" 等
                var roleStr = msg.Role.ToString().ToLower();
                var msgType = roleStr switch
                {
                    "user" => MessageType.User,
                    "assistant" => MessageType.Assistant,
                    "system" => MessageType.System,
                    _ => MessageType.Assistant
                };

                // 获取文本内容
                if (string.IsNullOrWhiteSpace(msg.Text)) continue;

                session.AddMessage(msg.Text, msgType);
                hasNewMessage = true;
            }

            if (hasNewMessage)
            {
                repo.Update(session);
                await repo.SaveChangesAsync(cancellationToken);
            }
        }

        public override async ValueTask<IEnumerable<ChatMessage>> InvokingAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
            if (this.SessionDbKey is null)
            {
                // No session key yet, so no messages to retrieve
                return [];
            }
            using var scope = _serviceProvider.CreateScope();
            var queryService = scope.ServiceProvider.GetRequiredService<IDataQueryService>();

            // 先按时间倒序(Descending)取最新的 50 条
            var queryable = queryService.Messages
                .Where(m => m.SessionId == SessionDbKey)
                .OrderByDescending(m => m.CreatedAt)
                .Take(50);

            var dbMessages = await queryService.ToListAsync(queryable);

            // 在内存中反转回正序(Ascending)，因为语言需要按时间顺序阅读
            var orderedMessages = dbMessages.OrderBy(m => m.CreatedAt);

            // 将实体转换为 Agent 框架的 ChatMessage
            var chatMessages = new List<ChatMessage>();

            foreach (var msg in orderedMessages)
            {
                var role = msg.Type switch
                {
                    MessageType.User => ChatRole.User,
                    MessageType.Assistant => ChatRole.Assistant,
                    MessageType.System => ChatRole.System,
                    _ => ChatRole.User
                };
                chatMessages.Add(new ChatMessage(role, msg.Content));
            }

            return chatMessages;
        }

        public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null)
        {
          return  JsonSerializer.SerializeToElement(this.SessionDbKey);
        }
    }
}
