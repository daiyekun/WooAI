using Microsoft.AspNetCore.Mvc;
using Dev.WooAI.HttpApi.Infrastructure;
using Dev.WooAI.HttpApi.Models;
using Dev.WooAI.IdentityService.Commands;

namespace Dev.WooAI.HttpApi.Controllers;

[Route("/api/identity")]
public class IdentityController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterRequest request)
    {
        var result = await Sender.Send(new CreateUserCommand(request.Username, request.Password));

        return ReturnResult(result);
    }
}