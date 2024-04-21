using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChallengeApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;

public class GetUserGroupTabQuery : IRequest<UserGroupVm>
{
}

public class GetUserGroupTabQueryHandler : IRequestHandler<GetUserGroupTabQuery, UserGroupVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetUserGroupTabQueryHandler(IMapper mapper, IApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<UserGroupVm> Handle(GetUserGroupTabQuery request, CancellationToken cancellationToken)
    {
        List<UserGroupDto> userGroups = await _context.UserUserGroups
        .Include(item => item.Group)
        .ProjectTo<UserGroupDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);

        foreach (var group in userGroups)
        {
            var user = _userService.GetUserById(group.UserId);
            if (user != null && user.Result != null)
            {
              group.User = new UserInfoDto {
                FirstName = user.Result.FirstName,
                LastName = user.Result.LastName,
              };
            }
        }


        var userGroup = new UserGroupVm {
            Administrator = userGroups.Where(item => item.GroupName == "Administrator").ToList(),
            Client = userGroups.Where(item => item.GroupName == "Client").ToList()
        };

        return userGroup;
    }
}

