using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Commands.LanguageModels.Commands;

[AuthorizeRequirement("AiGateway.DeleteLanguageModel")]
public record DeleteLanguageModelCommand(Guid Id) : ICommand<Result>;
    
public class DeleteLanguageModelCommandHandler(IRepository<LanguageModel> repo) 
    : ICommandHandler<DeleteLanguageModelCommand, Result>
{
    public async Task<Result> Handle(DeleteLanguageModelCommand request, CancellationToken cancellationToken)
    {
        var result = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (result == null) return Result.Success();
        
        repo.Delete(result);
        await repo.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}