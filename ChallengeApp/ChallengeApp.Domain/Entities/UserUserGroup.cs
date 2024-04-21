using ChallengeApp.Domain.Common;

namespace ChallengeApp.Domain.Entities;

public class UserUserGroup : BaseAuditableEntity
{
    public long UserGroupId { get; set; }
    public string UserId { get; set; }

    public UserGroup Group { get; private set; }

}
