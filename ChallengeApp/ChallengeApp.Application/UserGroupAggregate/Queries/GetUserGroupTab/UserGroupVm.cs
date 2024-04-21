namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;

public class UserGroupVm
{
    public IList<UserGroupDto> Administrator { get; set; }
    public IList<UserGroupDto>  Valuer { get; set; }
    public IList<UserGroupDto>  Client { get; set; }
}
