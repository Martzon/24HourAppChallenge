using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ChallengeApp.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            // TODO: Seed User Groups
            // TODO: Ensure administrator@localhost.com is Administrator
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Role for user
        var userRole = new IdentityRole("User");
        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }


        // Role for client
        var clientRole = new IdentityRole("Client");
        if (_roleManager.Roles.All(r => r.Name != clientRole.Name))
        {
            await _roleManager.CreateAsync(clientRole);
        }


        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@gmail.com", Email = "administrator@gmail.com", FirstName = "admin", LastName = "admin", IsActive = true };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name, userRole.Name });
            }
        }

        // Default Client User
        var clientUser = new ApplicationUser { UserName = "client@gmail.com", Email = "client@gmail.com", FirstName = "aag", LastName = "aag", IsActive = true };

        if (_userManager.Users.All(u => u.UserName != clientUser.UserName))
        {
            await _userManager.CreateAsync(clientUser, "ClientP@ssW0rd");
            if (!string.IsNullOrWhiteSpace(clientRole.Name))
            {
                await _userManager.AddToRolesAsync(clientUser, new[] { clientRole.Name });
            }
        }

        // Default Client User
        var clientUser2 = new ApplicationUser { UserName = "martzonm08@gmail.com", Email = "martzonm08@gmail.com", FirstName = "Martzon", LastName = "Munoz", IsActive = true };

        if (_userManager.Users.All(u => u.UserName != clientUser2.UserName))
        {
            await _userManager.CreateAsync(clientUser2, "ClientP@ssW0rd");
            if (!string.IsNullOrWhiteSpace(clientRole.Name))
            {
                await _userManager.AddToRolesAsync(clientUser2, new[] { clientRole.Name });
            }
        }


       
        if (!_context.UserGroups.Any())
        {
            _context.UserGroups.Add(new UserGroup
            {
                Name = "Administrator",
            });
            _context.UserGroups.Add(new UserGroup
            {
                Name = "Client",
            });

            await _context.SaveChangesAsync();
        }
        if (!_context.UserUserGroups.Any())
        {
            var adminGroup = _context.UserGroups.FirstOrDefault(e => e.Name == "Administrator");
            var admin1 = await _userManager.FindByEmailAsync("administrator@gmail.com");
            if (adminGroup != null && admin1 != null)
            {
                _context.UserUserGroups.Add(new UserUserGroup
                {
                    UserId = admin1.Id,
                    UserGroupId = adminGroup.Id
                });
                await _context.SaveChangesAsync();
            }

            var clientGroup = _context.UserGroups.FirstOrDefault(e => e.Name == "Client");
            var client1 = await _userManager.FindByEmailAsync("client@gmail.com");
            if (clientGroup != null && client1 != null)
            {
                _context.UserUserGroups.Add(new UserUserGroup
                {
                    UserId = client1.Id,
                    UserGroupId = clientGroup.Id
                });
                await _context.SaveChangesAsync();
            }


        }
    }
}