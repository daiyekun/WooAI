using Dev.WooAI.Core.Rag.Aggregates.EmbeddingModel;
using Dev.WooAI.Core.Rag.Aggregates.KnowledgeBase;

namespace Dev.WooAI.MigrationWorkApp.SeedData;

public static class RagData
{
    private static readonly Guid[] Guids =
    [
        Guid.NewGuid()
    ];

    public static IEnumerable<EmbeddingModel> EmbeddingModels()
    {
        var item1 = new EmbeddingModel(
            "mxbai-embed-large",
            "MokaAI",
            "http://localhost:11434/",	// Ollama   http://localhost:11434/api/embeddings
            "mxbai-embed-large", // LM Studio 中的名称
            1024, 
            32 * 1000)
        {
            Id = Guids[0]
        };

        return [item1];
    }

    public static IEnumerable<KnowledgeBase> KnowledgeBases()
    {
        var item1 = new KnowledgeBase("默认知识库", "系统默认知识库", Guids[0]);
        return [item1];
    }
}