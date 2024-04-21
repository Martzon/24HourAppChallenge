using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChallengeApp.Infrastructure.Identity;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using ChallengeApp.Application.UserAggregate.Queries;
using System.Security.Claims;
using C9.Standard.Application.Common.Models;

namespace ChallengeApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ApiControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly SignInManager<ApplicationUser> _signInManager;


    public AuthenticateController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IAuthService authService,
        IUserService userService)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _authService = authService;
        _userService = userService;
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest(new { message = "User does not exist." });

            if (model.Email != "administrator@localhost.com")
            {
                var userData = await Mediator.Send(new GetUserByEmail
                {
                    Email = user.Email
                });

                if (userData == null)
                    return BadRequest(new { message = "User does not exist." });

                if (!userData.IsActive)
                    return BadRequest(new { message = "User is Inactive." });
            }


            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {

                // Set claims role
                var userRoles = await _userManager.GetRolesAsync(user);

                var claimsAuth = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                        new Claim(ClaimTypes.Email, user.Email),
                    };

                foreach (var userRole in userRoles)
                {
                    claimsAuth.Add(new Claim(ClaimTypes.Role, userRole));
                }


                var token = _authService.CreateToken(claimsAuth, false);
                var longTermToken = _authService.CreateToken(claimsAuth, true);
                var refreshToken = _authService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                await _userManager.UpdateAsync(user);

                // Set claims identity
                var identity = new ClaimsIdentity(claimsAuth);
                HttpContext.User = new ClaimsPrincipal(identity);
                var tokenData = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    TwoFactorEnabled = false,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo,
                    LongTermToken = new JwtSecurityTokenHandler().WriteToken(longTermToken),
                    Name = string.IsNullOrEmpty(user.FirstName) ? user.Email : user.FirstName + " " + user.LastName
                });
            }
            else
            {
                return BadRequest(new { message = "Incorrect Password." });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("verify-token")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyToken([FromBody] LoginVerifyDto model)
    {
        var azureUser = User.FindFirst(ClaimTypes.Upn)?.Value;
        if (azureUser == null)
        {
            var userName = _authService.GetNameByToken(model.ApiToken);
            var userRole = await _userService.GetRolesByEmail(userName);

            if (userName != "administrator@localhost.com")
            {
                var userData = await Mediator.Send(new GetUserByEmail
                {
                    Email = userName
                });
                userData.UserRoles = userRole;

                return Ok(new
                {
                    userData.Id,
                    userData.UserName,
                    userData.Email,
                    userData.FirstName,
                    userData.LastName,
                    userData.IsActive,
                    userData.IsResetPasswordRequired,
                    userData.InitialPassword,
                    userData.Name,
                    userData.UserRoles,
                    userData.UserGroups,
                    userData.Status,
                    userData.UserRoleDisplay,
                    userData.UserGroupDisplay
                });
            }
            else
            {
                var user = await _userManager.FindByNameAsync(userName);
                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    UserRoles = userRole,
                    FirstName = "Administrator",
                    LastName = ""
                });
            }
        }
        else
        {

            var azureRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var azureName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new
            {
                userName = azureUser,
                Email = azureUser,
                UserRoles = new[] { azureRole },
                ProjectCount = 0
            });
        }

    }


    [Route("api/Me")]
    [HttpGet]
    public string GetUser()
    {
        return "Hello authenticated user! You UPN is: " + User.FindFirst(ClaimTypes.Upn)?.Value;
    }

    [HttpPost]
    [Route("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest(new { message = "User does not exist." });
        }

        if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime < DateTime.Now)
        {
            return BadRequest(new { message = "Invalid refresh token." });
        }

        // Generate new tokens
        var userRoles = await _userManager.GetRolesAsync(user);
        var claimsAuth = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                };

        foreach (var userRole in userRoles)
        {
            claimsAuth.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = _authService.CreateToken(claimsAuth, false);
        var longTermToken = _authService.CreateToken(claimsAuth, true);
        var newRefreshToken = _authService.GenerateRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update refresh token." });
        }

        var identity = new ClaimsIdentity(claimsAuth);
        HttpContext.User = new ClaimsPrincipal(identity);

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = newRefreshToken,
            Expiration = token.ValidTo,
            LongTermToken = new JwtSecurityTokenHandler().WriteToken(longTermToken),
            Name = string.IsNullOrEmpty(user.FirstName) ? user.Email : user.FirstName + " " + user.LastName
        });
    }


}
