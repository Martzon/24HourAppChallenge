using Microsoft.AspNetCore.Authorization;

namespace ChallengeApp.API.Common;
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params string[] roles) : base()
    {
        Roles = string.Join(",", roles);
    }
}