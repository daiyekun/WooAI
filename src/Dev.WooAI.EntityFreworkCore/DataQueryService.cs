using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.EntityFreworkCore;
using Dev.WooAI.Services.Common.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Dev.WooAI.EntityFrameworkCore;

public class DataQueryService(WooAiDbContext dbContext) : IDataQueryService
{
    public IQueryable<ConversationTemplate> ConversationTemplates => dbContext.ConversationTemplates.AsNoTracking();
    public IQueryable<LanguageModel> LanguageModels => dbContext.LanguageModels.AsNoTracking();
    public IQueryable<Session> Sessions => dbContext.Sessions.AsNoTracking();
    public IQueryable<Message> Messages => dbContext.Messages.AsNoTracking();
    
    public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable) where T : class
    {
        return await queryable.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable) where T : class
    {
        return await queryable.AsNoTracking().ToListAsync();
    }

    public async Task<bool> AnyAsync<T>(IQueryable<T> queryable) where T : class
    {
        return await queryable.AsNoTracking().AnyAsync();
    }
}