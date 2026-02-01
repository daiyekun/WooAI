using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Sessions.Commands;

[AuthorizeRequirement("AiGateway.DeleteSession")]
public record DeleteSessionCommand(Guid Id) : ICommand<Result>;
    
public class DeleteSessionCommandHandler(IRepository<Session> repo) 
    : ICommandHandler<DeleteSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var result = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (result == null) return Result.Success();
        
        repo.Delete(result);
        await repo.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}