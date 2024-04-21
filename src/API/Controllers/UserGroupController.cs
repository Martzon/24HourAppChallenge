using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.UserGroupAggregate.Commands.CreateUserGroup;
using ChallengeApp.Application.UserGroupAggregate.Commands.DeleteUserGroup;
using ChallengeApp.Application.UserGroupAggregate.Commands.UpdateUserGroup;
using ChallengeApp.Application.UserGroupAggregate.Queries.GetUserGroupPagination;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserGroupController : ApiControllerBase
{
    private readonly ILogger<UserGroupController> _logger;

    public UserGroupController(ILogger<UserGroupController> logger)
    {
        _logger = logger;
    }


    [HttpGet]
    [Route("pagination")]
    public async Task<ActionResult> GetUserGroupPagination([FromQuery] GetUserGroupPaginationQuery command)
    {
        try
        {
            return Ok(await Mediator.Send(command));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserGroupCommand query)
    {
        try
        {
            return Ok(await Mediator.Send(query));
        }
        catch (NotFoundException ex)
        {
            return Handle(ex);
        }
        catch (AlreadyExistsException ex)
        {
            return Handle(ex);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserGroupCommand query)
    {
        try
        {
            return Ok(await Mediator.Send(query));
        }
        catch (NotFoundException ex)
        {
            return Handle(ex);
        }
        catch (AlreadyExistsException ex)
        {
            return Handle(ex);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] DeleteUserGroupCommand query)
    {
        try
        {
            return Ok(await Mediator.Send(query));
        }
        catch (NotFoundException ex)
        {
            return Handle(ex);
        }
        catch (AlreadyExistsException ex)
        {
            return Handle(ex);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

}