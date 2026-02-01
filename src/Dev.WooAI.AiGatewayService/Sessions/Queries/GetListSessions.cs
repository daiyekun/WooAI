using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Contracts;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.Sessions.Queries;

public record SessionDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
}

[AuthorizeRequirement("AiGateway.GetListSessions")]
public record GetListSessionsQuery : IQuery<Result<IList<SessionDto>>>;

public class GetListSessionsQueryHandler(
    IDataQueryService dataQueryService) : IQueryHandler<GetListSessionsQuery, Result<IList<SessionDto>>>
{
    public async Task<Result<IList<SessionDto>>> Handle(GetListSessionsQuery request, CancellationToken cancellationToken)
    {
        var queryable = dataQueryService.Sessions
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Title = s.Title
            });
        var result= await dataQueryService.ToListAsync(queryable);
        return Result.Success(result);
    }
}