namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;

public class UserInfoDto 
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name { 
        get {
            return FirstName + " " + LastName;
        }
    }
}
