namespace ChallengeApp.Application.Common.Models
{
    public class UserAccountDto
    {
        public UserAccountDto(string id, string userName, string email, string firstName, string lastName, bool isActive, List<string> userGroups)
        {
            Id = id;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsActive = isActive;
            UserGroups = userGroups;
        }


        public string Id { get; set; } 
        public string UserName { get; set; } 
        public string Email { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public bool IsActive { get; set; } 
        public string Name { get { return $"{FirstName} {LastName}"; } }
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
}
