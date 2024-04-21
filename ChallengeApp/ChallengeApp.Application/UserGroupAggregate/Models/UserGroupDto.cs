using ChallengeApp.Application.Common.Mappings;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.UserGroupAggregate.Models
{
    public class UserGroupDto : IMapFrom<UserGroup>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
