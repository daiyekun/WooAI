using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Core.Rag.Aggregates.EmbeddingModel;
using Dev.WooAI.Core.Rag.Aggregates.KnowledgeBase;
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

    // RAG 实体模型
    public DbSet<EmbeddingModel> EmbeddingModels => Set<EmbeddingModel>();
    public DbSet<KnowledgeBase> KnowledgeBases => Set<KnowledgeBase>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}


