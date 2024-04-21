using ChallengeApp.Application.Common.Mappings;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupPagination;

public class UserGroupDto : IMapFrom<UserGroup>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<UserUserGroup> Members { get; set; }
}
