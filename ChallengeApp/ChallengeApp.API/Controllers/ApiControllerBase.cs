using ChallengeApp.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected virtual IActionResult Handle(Exception ex)
    {
        switch (ex)
        {
            case NotFoundException:
                return StatusCode(StatusCodes.Status404NotFound, new { message = ex.Message });
            case AlreadyExistsException:
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            case InvalidOperationException:
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Unexpected thing happened. Sorry." });
            case UnauthorizeException:
                return StatusCode(StatusCodes.Status401Unauthorized,
                    new { message = "Auth Expired" });
            default:
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
        }
    }
}