using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Dev.WooAI.EntityFreworkCore;

public class WooAiDbContext(DbContextOptions<WooAiDbContext> options) : IdentityDbContext(options)
{
    // AiGateway 实体模型
    public DbSet<LanguageModel> LanguageModels => Set<LanguageModel>();
    public DbSet<ConversationTemplate> ConversationTemplates => Set<ConversationTemplate>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}


