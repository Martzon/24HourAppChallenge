namespace ChallengeApp.Application.Common.Models;

public class UserAccount
{
    public UserAccount() { }

    public UserAccount(string userName, string email, string firstName, string lastName, bool isActive, bool isResetPasswordRequired, string initialPassword)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = isActive;
        IsResetPasswordRequired = isResetPasswordRequired;
        InitialPassword = initialPassword;
    }

    public UserAccount(string userName, string email, string firstName, string lastName, bool isActive)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = isActive;
    }

    public UserAccount(string userName, string email, string firstName, string lastName, bool isActive, List<string> userGroups)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = isActive;
        UserGroups = userGroups;
    }

    public UserAccount(string userId, string userName, string email, string firstName, string lastName, bool isActive, List<string> userGroups)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = isActive;
        UserGroups = userGroups;
        Id = userId;
    }

    public UserAccount(string id)
    {
        Id = id;
    }
    public string Id { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = false;
    public bool IsResetPasswordRequired { get; private set; } = false;
    public string InitialPassword { get; private set; } = string.Empty;
    public string Name { get { return $"{FirstName} {LastName}"; } }
    public void UpdateChangePasswordRequired(bool isResetPasswordRequired)
    {
        IsResetPasswordRequired = isResetPasswordRequired;
    }


    public List<string> UserRoles { get; set; } = new List<string>();
    public List<string> UserGroups { get; set; } = new List<string>();
    public string Status
    {
        get
        {
            return IsActive ? "Active" : "Inactive";
        }
    }

    public string UserRoleDisplay
    {
        get
        {
            return string.Join(", ", UserRoles);
        }
    }

    public string UserGroupDisplay
    {
        get
        {
            return string.Join(", ", UserGroups);
        }
    }
}
