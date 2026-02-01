using Dev.WooAI.Core.AiGateway.Aggregates.Sessions;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Contracts;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Sessions.Commands;

public record CreatedSessionDto(Guid Id);

[AuthorizeRequirement("AiGateway.CreateSession")]
public record CreateSessionCommand(Guid TemplateId) : ICommand<Result<CreatedSessionDto>>;
    
public class CreateSessionCommandHandler(IRepository<Session> repo, ICurrentUser user) 
    : ICommandHandler<CreateSessionCommand, Result<CreatedSessionDto>>
{
    public async Task<Result<CreatedSessionDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var result = new Session(new Guid(user.Id!), request.TemplateId);
        
        repo.Add(result);

        await repo.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new CreatedSessionDto(result.Id));
    }
}