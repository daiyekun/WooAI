using Dev.WooAI.Core.AiGateway.Aggregates.LanguageModel;
using Dev.WooAI.Services.Common.Attributes;
using Dev.WooAI.SharedKernel.Messaging;
using Dev.WooAI.SharedKernel.Repository;
using Dev.WooAI.SharedKernel.Result;

namespace Dev.WooAI.AiGatewayService.LanguageModels.Commands;

public record CreatedLanguageModelDto(Guid Id, string Provider, string Name);

[AuthorizeRequirement("AiGateway.CreateLanguageModel")]
public record CreateLanguageModelCommand(
    string Provider, 
    string Name, 
    string BaseUrl, 
    string? ApiKey,
    int MaxTokens,
    double Temperature = 0.7) : ICommand<Result<CreatedLanguageModelDto>>;
    
public class CreateLanguageModelCommandHandler(IRepository<LanguageModel> repo) 
    : ICommandHandler<CreateLanguageModelCommand, Result<CreatedLanguageModelDto>>
{
    public async Task<Result<CreatedLanguageModelDto>> Handle(CreateLanguageModelCommand request, CancellationToken cancellationToken)
    {
        var result = new LanguageModel(
            request.Name, 
            request.Provider,
            request.BaseUrl,
            request.ApiKey,
            new ModelParameters 
            {
                MaxTokens = request.MaxTokens,
                Temperature = request.Temperature 
            });
        
        repo.Add(result);

        await repo.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new CreatedLanguageModelDto(result.Id, result.Provider, result.Name));
    }
}