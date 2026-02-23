using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Core.Rag.Aggregates.EmbeddingModel;
using Dev.WooAI.Core.Rag.Aggregates.KnowledgeBase;
using System.Reflection.Metadata;
using Document = Dev.WooAI.Core.Rag.Aggregates.KnowledgeBase.Document;

namespace Dev.WooAI.Services.Common.Contracts;

public interface IDataQueryService
{
    public IQueryable<ConversationTemplate> ConversationTemplates { get; }

    public IQueryable<LanguageModel> LanguageModels { get; }

    public IQueryable<Session> Sessions { get; }

    public IQueryable<Message> Messages { get; }

    public IQueryable<EmbeddingModel> EmbeddingModels { get; }
    public IQueryable<KnowledgeBase> KnowledgeBases { get; }
    public IQueryable<Document> Documents { get; }
    public IQueryable<DocumentChunk> DocumentChunks { get; }

    Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable) where T : class;

    Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable) where T : class;

    Task<bool> AnyAsync<T>(IQueryable<T> queryable) where T : class;
}
