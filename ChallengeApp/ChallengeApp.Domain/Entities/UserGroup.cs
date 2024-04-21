using ChallengeApp.Domain.Common;

namespace ChallengeApp.Domain.Entities;

public class UserGroup : BaseAuditableEntity
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual List<UserUserGroup> Members { get; set; }
}