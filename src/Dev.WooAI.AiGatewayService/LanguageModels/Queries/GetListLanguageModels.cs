using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.Services.Contracts;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.LanguageModels.Queries;

public record LanguageModelDto
{
    public Guid Id { get; set; }
    public required string Provider { get; set; }
    public required string Name { get; set; }
    public required string BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public int MaxTokens { get; set; }
    public double Temperature { get; set; }
}

[AuthorizeRequirement("AiGateway.GetListLanguageModels")]
public record GetListLanguageModelsQuery : IQuery<Result<IList<LanguageModelDto>>>;

public class GetListLanguageModelsQueryHandler(
    IDataQueryService dataQueryService) : IQueryHandler<GetListLanguageModelsQuery, Result<IList<LanguageModelDto>>>
{
    public async Task<Result<IList<LanguageModelDto>>> Handle(GetListLanguageModelsQuery request, CancellationToken cancellationToken)
    {
        var queryable = dataQueryService.LanguageModels
            .Select(lm => new LanguageModelDto
            {
                Id = lm.Id,
                Provider = lm.Provider,
                Name = lm.Name,
                BaseUrl = lm.BaseUrl,
                ApiKey = lm.ApiKey,
                MaxTokens = lm.Parameters.MaxTokens,
                Temperature = lm.Parameters.Temperature
            });
        var result= await dataQueryService.ToListAsync(queryable);
        return Result.Success(result);
    }
}