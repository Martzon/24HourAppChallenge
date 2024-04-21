namespace ChallengeApp.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string[]? Roles { get; }
    string? Id { get; }
}
