using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Commands.ConversationTemplates.Commands;

public record CreatedConversationTemplateDto(Guid Id, string Name);

[AuthorizeRequirement("AiGateway.CreateConversationTemplate")]
public record CreateConversationTemplateCommand(
    string Name, 
    string Description, 
    string SystemPrompt,
    Guid ModelId,
    int? MaxTokens,
    float? Temperature) : ICommand<Result<CreatedConversationTemplateDto>>;
    
public class CreateConversationTemplateCommandHandler(IRepository<ConversationTemplate> repo) 
    : ICommandHandler<CreateConversationTemplateCommand, Result<CreatedConversationTemplateDto>>
{
    public async Task<Result<CreatedConversationTemplateDto>> Handle(CreateConversationTemplateCommand request, CancellationToken cancellationToken)
    {
        var model = new ConversationTemplate(
            request.Name, 
            request.Description,
            request.SystemPrompt,
            request.ModelId,
            new TemplateSpecification 
            {
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature 
            });
        
        repo.Add(model);

        await repo.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new CreatedConversationTemplateDto(model.Id, model.Name));
    }
}