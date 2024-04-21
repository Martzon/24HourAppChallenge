
using ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;

namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupPagination;

public class UserGroupVm
{
    public List<UserGroupItem> Items  {get; set; }
    public int PageNumber { set; get; }
    public int TotalPages { set; get; }
    public int TotalCount { set; get; }
}
public class UserGroupItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<UserInfoDto> Members { get; set; }
    
    public string Status 
    {
        get {
            return IsActive ? "Active" : "Inactive";
        }
    }
}
