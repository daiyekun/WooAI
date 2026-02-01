using Dev.WooAI.AiGatewayService.ConversationTemplates.Dtos;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Contracts;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.ConversationTemplates.Queries;

[AuthorizeRequirement("AiGateway.GetConversationTemplate")]
public record GetConversationTemplateQuery(Guid Id) : IQuery<Result<ConversationTemplateDto>>;

public class GetConversationTemplateQueryHandler(
    IDataQueryService dataQueryService) : IQueryHandler<GetConversationTemplateQuery, Result<ConversationTemplateDto>>
{
    public async Task<Result<ConversationTemplateDto>> Handle(GetConversationTemplateQuery request, CancellationToken cancellationToken)
    {
        var queryable = dataQueryService.ConversationTemplates
            .Where(template => template.Id == request.Id)
            .Select(ct => new ConversationTemplateDto
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description,
                SystemPrompt = ct.SystemPrompt,
                MaxTokens = ct.Specification.MaxTokens,
                Temperature = ct.Specification.Temperature
            });
        var result= await dataQueryService.FirstOrDefaultAsync(queryable);
        
        return result == null ? Result.NotFound() : Result.Success(result);
    }
}