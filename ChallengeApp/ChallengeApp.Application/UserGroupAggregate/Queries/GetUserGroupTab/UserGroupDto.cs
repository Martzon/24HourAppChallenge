using ChallengeApp.Application.Common.Mappings;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;

public class UserGroupDto : IMapFrom<UserUserGroup>
{
    public long UserGroupId { get; set; }
    public string UserId { get; set; }
    public string GroupName { get; private set; }
    public UserInfoDto User { get; set; }
}
