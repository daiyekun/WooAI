using Microsoft.AspNetCore.Identity;

namespace Dev.WooAI.IdentityService.Contracts;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}