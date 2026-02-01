using Dev.WooAI.Core.AiGateway.Aggregates.ConversationTemplate;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.ConversationTemplates.Commands;

[AuthorizeRequirement("AiGateway.DeleteConversationTemplate")]
public record DeleteConversationTemplateCommand(Guid Id) : ICommand<Result>;
    
public class DeleteConversationTemplateCommandHandler(IRepository<ConversationTemplate> modelRepo) 
    : ICommandHandler<DeleteConversationTemplateCommand, Result>
{
    public async Task<Result> Handle(DeleteConversationTemplateCommand request, CancellationToken cancellationToken)
    {
        var model = await modelRepo.GetByIdAsync(request.Id, cancellationToken);
        if (model == null) return Result.Success();
        
        modelRepo.Delete(model);
        await modelRepo.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}