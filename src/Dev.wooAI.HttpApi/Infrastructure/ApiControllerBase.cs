using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dev.WooAI.SharedKernel.Result;
using IResult = Dev.WooAI.SharedKernel.Result.IResult;

namespace Dev.WooAI.HttpApi.Infrastructure;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    public ISender Sender => HttpContext.RequestServices.GetRequiredService<ISender>();

    [NonAction]
    public IActionResult ReturnResult(IResult result)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
            {
                var value = result.GetValue();
                return value is null ? NoContent() : Ok(value);
            }
            case ResultStatus.Error:
                return result.Errors is null ? BadRequest() : BadRequest(new { errors = result.Errors });

            case ResultStatus.NotFound:
                return result.Errors is null ? NotFound() : NotFound(new { errors = result.Errors });

            case ResultStatus.Invalid:
                return result.Errors is null ? BadRequest() : BadRequest(new { errors = result.Errors });

            case ResultStatus.Forbidden:
                return StatusCode(403);

            case ResultStatus.Unauthorized:
                return Unauthorized();

            default:
                return BadRequest();
        }
    }
}