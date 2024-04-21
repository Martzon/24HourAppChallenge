using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using ChallengeApp.Domain.Common;

namespace ChallengeApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser, IHasDomainEvent
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
