namespace ChallengeApp.Application.Common.Interfaces;

public interface IUserService
{
    Task<List<UserAccount>> GetUserByIds(List<string> userIds);
    Task<UserAccount> GetUserById(string id);
    Task<UserAccount> GetUserByEmail(string email);
    Task<List<UserAccount>> GetUserByKeyword(string keyword);
    Task<PaginatedList<UserAccountDto>> GetUserWithPagination(string? keyword, int take, int skip, string? sort);
    Task<(Result Result, string UserId)> CreateUserAsync(UserAccount user, string password);
    Task<(Result Result, string UserId)> UpdateUserAsync(UserAccount user);
    Task DeleteUserAsync(UserAccount user);
    Task CreateRolesAsync(string userId, List<string> roles);
    Task UpdateRolesAsync(string userId, List<string> roles);
    Task<List<string>> GetRolesByEmail(string Email);
    Task<string> GenerateResetPasswordToken(string Email);
    Task<PaginatedList<RoleModel>> GetRoles(string name, string sort, int skip, int take);
    Task<List<RoleModel>> GetAllRoles();
    Task CreateRoleAsync(string name);
    Task UpdateRoleAsync(string id, string name);
    Task DeleteRoleAsync(string id);
}