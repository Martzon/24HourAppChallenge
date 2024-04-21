namespace ChallengeApp.Application.Common.Models;
public class LoginModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool IsRemberMe { get; set; }
}