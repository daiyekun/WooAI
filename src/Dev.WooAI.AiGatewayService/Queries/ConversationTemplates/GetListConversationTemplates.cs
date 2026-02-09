
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Common.Contracts;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Queries.ConversationTemplates;

[AuthorizeRequirement("AiGateway.GetListConversationTemplates")]
public record GetListConversationTemplatesQuery : IQuery<Result<IList<ConversationTemplateDto>>>;

public class GetListConversationTemplatesQueryHandler(
    IDataQueryService dataQueryService) : IQueryHandler<GetListConversationTemplatesQuery, Result<IList<ConversationTemplateDto>>>
{
    public async Task<Result<IList<ConversationTemplateDto>>> Handle(GetListConversationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var queryable = dataQueryService.ConversationTemplates
            .Select(ct => new ConversationTemplateDto
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description,
                SystemPrompt = ct.SystemPrompt,
                MaxTokens = ct.Specification.MaxTokens,
                Temperature = ct.Specification.Temperature
            });
        var result= await dataQueryService.ToListAsync(queryable);
        return Result.Success(result);
    }
}