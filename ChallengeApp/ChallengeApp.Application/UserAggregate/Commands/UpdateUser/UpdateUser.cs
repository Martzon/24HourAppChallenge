using MediatR;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ChallengeApp.Domain.Enums;

namespace ChallengeApp.Application.UserAggregate.Commands.UpdateUser;
public class UpdateUser : IRequest<string>
{
    public string Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public List<string> UserGroups { get; set; } = null!;
}

public class UpdateUserHandler : IRequestHandler<UpdateUser, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserService _userService;

    public UpdateUserHandler(IApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<string> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        var userExists = await _userService.GetUserByEmail(request.Email);
        if (userExists == null)
            throw new NotFoundException("User does not exists.");

        var user = new UserAccount(request.Email, request.Email, request.FirstName, request.LastName, request.IsActive);

        var (identityResult, userId) = await _userService.UpdateUserAsync(user);

        if (identityResult.Succeeded)
        {
            //Default role user
            var defaultRoles = new List<string>();
            defaultRoles.Add(UserRoles.User.ToString());
            if (request.UserGroups.Contains(UserRoles.Admin.ToString()))
            {
                defaultRoles.Add(UserRoles.Admin.ToString());
            }
            await _userService.UpdateRolesAsync(userId, defaultRoles);
            

            var currentUserGroupIds = _context.UserUserGroups.Where(e => e.UserId == userId).Select(e => e.UserGroupId).ToList();

            // ** User Group ** //
            var userGroupIds = _context.UserGroups.Where(e => request.UserGroups.Contains(e.Name)).Select(g => g.Id).ToList();
            var excludedUserGroups= currentUserGroupIds.Except(userGroupIds);
            var addUserGroupIds = userGroupIds.Except(currentUserGroupIds);


            var removeUserAssignedGroups = _context.UserUserGroups.Where(e => e.UserId == userId && excludedUserGroups.Contains(e.UserGroupId)).ToArray();
            _context.UserUserGroups.RemoveRange(removeUserAssignedGroups);

         
            foreach (long userGroupId in addUserGroupIds)
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
