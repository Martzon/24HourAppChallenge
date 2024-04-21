using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(Policy = AuthenticationSchemes.JwtAndAzureAd, Roles = "Administrator")]
    public class UserController : ApiControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("getuserswithpagination")]
        public async Task<IActionResult> Get([FromBody] GetUsersWithPagination command)
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetUsers command)
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

        [HttpGet("getavailableusers")]
        public async Task<IActionResult> GetAvailableUsers([FromQuery] GetUsers command)
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
        // [AuthorizeRoles(UserRoles.Admin)]
        public async Task<IActionResult> Create(CreateUser command)
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

        [HttpPut]
        // [AuthorizeRoles(UserRoles.Admin)]
        public async Task<IActionResult> Update(UpdateUser command)
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

        [HttpDelete]
        [AuthorizeRoles(UserRoles.Admin)]
        public async Task<IActionResult> Delete([FromQuery] DeleteUser command)
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
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> Create(ForgotPassword command)
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
        [Route("update-changepassword-required")]
        public async Task<IActionResult> UpdateChangePasswordRequired(UpdateChangePasswordRequired command)
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

        [HttpGet]
        [Route("getuserbyemail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] GetUserByEmail command)
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
        [Route("createclient")]
        public async Task<IActionResult> CreateUserClient([FromQuery] CreateUser command)
        {
            try
            {
                // Role for client
                var clientRole = new IdentityRole("Client");

                // Default users
                var client = new ApplicationUser { UserName = command.Email, Email = command.Email, IsActive = true };

                if (_userManager.Users.All(u => u.UserName != client.UserName))
                {
                    var result = await _userManager.CreateAsync(client, command.Password);
                    if (result.Errors.Count() > 0)
                    {
                        var error = result.Errors.First();
                        return BadRequest(error.Description);
                    }
                    if (!string.IsNullOrWhiteSpace(clientRole.Name))
                    {
                        await _userManager.AddToRolesAsync(client, new[] { clientRole.Name });
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("usergroups")]
        public async Task<IActionResult> GetUserGroups([FromQuery] GetUserGroupOptions command)
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

        [HttpGet]
        [Route("getusergrouptab")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserGroupTab([FromQuery] GetUserGroupTabQuery command)
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

    }
}
