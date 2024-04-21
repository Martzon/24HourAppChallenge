using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using ChallengeApp.Infrastructure.Identity;

namespace C9.Standard.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public UserService(IApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<UserAccount> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user != null)
        {
            var userGroups = await _context.UserUserGroups.Include(e => e.Group).Where(e => e.UserId == user.Id).Select(e => e.Group.Name).ToListAsync();

            return new UserAccount(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                userGroups);
        }

        return null;
    }

    public async Task<UserAccount> GetAllUsersWithGroup(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user != null)
        {
            var userGroups = await _context.UserUserGroups.Include(e => e.Group).Where(e => e.UserId == user.Id).Select(e => e.Group.Name).ToListAsync();

            return new UserAccount(
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                userGroups);
        }

        return null;
    }


    public async Task<UserAccount> GetUserByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user != null)
        {
            var userGroups = await _context.UserUserGroups.Include(e => e.Group).Where(e => e.UserId == user.Id).Select(e => e.Group.Name).ToListAsync();
            return new UserAccount(
                user.Id,
                user.UserName, 
                user.Email, 
                user.FirstName, 
                user.LastName, 
                user.IsActive,
                userGroups);
        }

        return null;
    }

    public async Task<List<UserAccount>> GetUserByIds(List<string> userIds)
    {
        List<UserAccount> userList = new List<UserAccount>();
        var users = await _userManager.Users
                           .Where(item => userIds.Contains(item.Id))
                           .Select(user=> new UserAccount(
                                user.Id,
                                user.UserName,
                                user.Email,
                                user.FirstName,
                                user.LastName,
                                user.IsActive, null))
                           .ToListAsync();
        return users;
    }

    public async Task<List<UserAccount>> GetUserByKeyword(string keyword)
    {
        List<UserAccount> userList = new List<UserAccount>();
        var users = await _userManager.Users
                           .Where(item => string.IsNullOrEmpty(keyword)
                            || item.UserName.Contains(keyword)
                            || item.FirstName.Contains(keyword)
                            || item.LastName.Contains(keyword)
                            || (item.FirstName + " " + item.LastName).Contains(keyword)
                            )
                           .OrderBy(o => o.FirstName).ThenBy(o => o.LastName)
                           .Where(item => item.IsActive)
                           .ToListAsync();

        var userGroups = await _context.UserUserGroups.Include(e => e.Group).ToListAsync();

        if (users != null && users.Count > 0)
        {
            foreach (var user in users)
            {
                var userGroupList = userGroups.Where(e => e.UserId == user.Id).Select(e => e.Group.Name).ToList();
                userList.Add(
                    new UserAccount(
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.IsActive,
                    userGroupList));
            }
        }

        return userList;
    }

    public async Task<PaginatedList<UserAccountDto>> GetUserWithPagination(string? keyword, int take, int skip, string? sort)
    {
        List<UserAccountDto> userList = new List<UserAccountDto>();

        var value = keyword?.ToLower().Trim();

        var query = _userManager.Users
        .Where(item => string.IsNullOrEmpty(value)
            || item.UserName.ToLower().Trim().Contains(value)
            || item.PhoneNumber == value
            || item.FirstName.ToLower().Trim().Contains(value)
            || item.LastName.ToLower().Trim().Contains(value)
            || (item.FirstName.ToLower().Trim() + " " + item.LastName.ToLower().Trim()).Contains(value));

        var sortMappings = new Dictionary<string, string>
        {
            {"id", "Id"},
            {"status", "IsActive"},
            {"firstName", "FirstName"},
            {"lastName", "LastName"},
            {"name", "FirstName"},
            {"email", "Email"},
            {"phoneNumber", "PhoneNumber"},
            {"created", "Created"},
            {"lastModified", "LastModified"},
        };

        // Parse the sort string
        string[] sortParts = sort.Split(' ');
        string sortKey = sortParts[0];
        string sortOrder = sortParts.Length > 1 ? sortParts[1] : "asc";

        if (sortMappings.TryGetValue(sortKey, out var propertyName))
        {
            var property = typeof(ApplicationUser).GetProperty(propertyName);

            if (property != null)
            {
                var parameter = Expression.Parameter(typeof(ApplicationUser), "x");
                var propertyExpression = Expression.Property(parameter, property);
                var lambda = Expression.Lambda(propertyExpression, parameter);

                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    query = Queryable.OrderBy(query, (dynamic)lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    query = Queryable.OrderByDescending(query, (dynamic)lambda);
                }
            }
        }

        var users = await query.ToListAsync();
        var userGroups = await _context.UserUserGroups.Include(e => e.Group).ToListAsync();



        if (users != null && users.Count > 0)
        {
            foreach (var user in users)
            {
                var userGroupList = userGroups.Where(e => e.UserId == user.Id).Select(e => e.Group.Name).ToList();

                userList.Add(
                    new UserAccountDto(
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.IsActive,
                    userGroupList));
            }
        }

        var userListResult = userList.ToList();

        if (sortKey.Contains("userGroupDisplay"))
        {
            if (sortOrder == "desc")
            {
                userList = userList.OrderByDescending(x => x.UserGroupDisplay).ToList();
            }
            else
            {
                userList = userList.OrderBy(x => x.UserGroupDisplay).ToList();
            }
        }

        var pageNumber = skip == 0 ? 1 : (userList.Count() / skip) + 1;
        var count = (skip < userList.Count()) ? skip : 0;

        return new PaginatedList<UserAccountDto>(
            userList.Skip(count).Take(take).ToList(),
            userList.Count(),
            pageNumber,
            take
        );
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(UserAccount user, string password)
    {
        var newuser = new ApplicationUser
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,

        };
        var findUser = await _userManager.FindByEmailAsync(user.Email);
        if (findUser != null)
            return (Result.Success(), findUser.Id.ToString());

        var result = await _userManager.CreateAsync(newuser, password);
        
        
        return (result.ToApplicationResult(), newuser.Id);
    }

    public async Task<(Result Result, string UserId)> UpdateUserAsync(UserAccount user)
    {
        var findUser = await _userManager.FindByEmailAsync(user.Email);
        findUser.Email = user.Email;
        findUser.UserName = user.UserName;
        findUser.IsActive = user.IsActive;
        findUser.FirstName = user.FirstName;
        findUser.LastName = user.LastName;


        await _userManager.UpdateAsync(findUser);
        return (Result.Success(), findUser.Id.ToString());
    }

    public async Task DeleteUserAsync(UserAccount user)
    {
        var findUser = await _userManager.FindByEmailAsync(user.Email);
        await _userManager.DeleteAsync(findUser);
    }

    public async Task CreateRolesAsync(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var userRoles = await _userManager.GetRolesAsync(user);

        roles.Except(userRoles);
        await _userManager.AddToRolesAsync(user, roles);
    }

    public async Task UpdateRolesAsync(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var userRoles = await _userManager.GetRolesAsync(user);

        var excludedRoles = userRoles.Except(roles);
        var addRoles = roles.Except(userRoles);

        foreach (var userRole in excludedRoles)
        {
            await _userManager.RemoveFromRoleAsync(user, userRole);
        }

        await _userManager.AddToRolesAsync(user, addRoles);
    }

    public async Task<List<string>> GetRolesByEmail(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        var userRoles = await _userManager.GetRolesAsync(user);

        return userRoles.ToList();
    }

    public async Task<PaginatedList<RoleModel>> GetRoles(string name, string sort, int skip, int take)
    {
        var roles = await _roleManager.Roles.Select(s => new { ID = s.Id, s.Name }).ToListAsync();
        List<RoleModel> userRoles = new List<RoleModel>();
        var displayId = 1;
        foreach (var role in roles)
        {
            var userRole = new RoleModel() { Id = role.ID, Name = role.Name, DisplayId = displayId };
            displayId += 1;
            userRoles.Add(userRole);
        }
        var allroles = userRoles.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));
        if (sort == "name asc")
        {
            allroles = allroles.OrderBy(e => e.Name);
        }
        if (sort == "name desc")
        {
            allroles = allroles.OrderByDescending(e => e.Name);
        }
        if (sort == "id desc")
        {
            allroles = allroles.OrderByDescending(e => e.Id);
        }
        // return allroles.ToList();
        if (skip >= allroles.Count()) skip = 0;
        return new PaginatedList<RoleModel>(
            allroles.Skip(skip).ToList(),
            allroles.Count(),
            roles.Count(),
            allroles.Count()
        );
    }

    public async Task<List<RoleModel>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(s => new { ID = s.Id, s.Name }).ToListAsync();
        List<RoleModel> userRoles = new List<RoleModel>();

        foreach (var role in roles)
        {
            var userRole = new RoleModel() { Id = role.ID, Name = role.Name };
            userRoles.Add(userRole);
        }

        return userRoles;
    }

    public async Task CreateRoleAsync(string name)
    {
        var administratorRole = new IdentityRole(name);
        var roles = await _roleManager.Roles.Select(s => new { ID = s.Id, s.Name }).ToListAsync();
        List<RoleModel> userRoles = new List<RoleModel>();

        foreach (var role in roles)
        {
            var userRole = new RoleModel() { Id = role.ID, Name = role.Name };
            userRoles.Add(userRole);
        }
        var allroles = userRoles.Where(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).ToList();
        if (allroles.Count >= 1)
        {
            throw new AlreadyExistsException("User Role exists");
        }
        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }
    }

    public async Task UpdateRoleAsync(string id, string name)
    {
        var roles = await _roleManager.Roles.Select(s => new { ID = s.Id, s.Name }).ToListAsync();
        foreach (var element in roles)
        {
            var userRole = new RoleModel() { Id = element.ID, Name = element.Name };
            if (element.ID != id && element.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new AlreadyExistsException("User Role exists");
            }
        }
        var role = await _roleManager.FindByIdAsync(id);
        role.Name = name;
        role.NormalizedName = name;
        await _roleManager.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        await _roleManager.DeleteAsync(role);
    }

}
