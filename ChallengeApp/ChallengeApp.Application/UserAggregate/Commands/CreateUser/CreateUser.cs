using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ChallengeApp.Application.UserAggregate.Commands.CreateUser;
public class CreateUser : IRequest<string>
{
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsActive { get; set; } = false;
    public bool IsResetPasswordRequired { get; set; } = false;
    public List<string> UserGroups { get; set; } = new List<string>();
    public string? Password { get; set; } = string.Empty;
}

public class CreateUserHandler : IRequestHandler<CreateUser, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserService _userService;
    

    public CreateUserHandler(IApplicationDbContext context, IIdentityService identityService, IUserService userService)
    {
        _context = context;
        _userService = userService;     
    }

    public async Task<string> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        var userExists =  await _userService.GetUserByEmail(request.Email);
        if (userExists != null)
            throw new AlreadyExistsException("User already exists");

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var initialPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: request.Email,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 32));

        
        var user = new UserAccount(request.Email, request.Email, request.FirstName, request.LastName, request.IsActive, request.IsResetPasswordRequired, initialPassword);


        var (identityResult, userId) = await _userService.CreateUserAsync(user, initialPassword);

        if (identityResult.Succeeded)
        {
            //Default role user
            var defaultRoles = new List<string>();
            defaultRoles.Add(UserRoles.User.ToString());
            if (request.UserGroups.Contains(UserRoles.Admin.ToString()))
            {
                defaultRoles.Add(UserRoles.Admin.ToString());
            }
            await _userService.CreateRolesAsync(userId, defaultRoles);

            //Assign user usergroups
            var userGroupIds = _context.UserGroups.Where(e => request.UserGroups.Contains(e.Name)).Select(g => g.Id).ToList();
            foreach (long userGroupId in userGroupIds)
            {
                _context.UserUserGroups.Add(new UserUserGroup
                {
                    UserId = userId,
                    UserGroupId = userGroupId
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

        }

        return userId;
        
    }
}
