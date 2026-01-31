using Microsoft.AspNetCore.Identity;

namespace Dev.WooAI.Services.Contracts;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}