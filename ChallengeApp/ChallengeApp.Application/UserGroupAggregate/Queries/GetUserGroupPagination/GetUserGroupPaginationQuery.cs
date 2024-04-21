using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Mappings;
using ChallengeApp.Application.UserGroupAggregate.GetUserGroupPagination;
using ChallengeApp.Application.UserGroupAggregate.GetUserGroupTab;
using ChallengeApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserGroupDto = ChallengeApp.Application.UserGroupAggregate.GetUserGroupPagination.UserGroupDto;
using UserGroupVm = ChallengeApp.Application.UserGroupAggregate.GetUserGroupPagination.UserGroupVm;

namespace ChallengeApp.Application.UserGroupAggregate.Queries.GetUserGroupPagination;

public class GetUserGroupPaginationQuery : IRequest<UserGroupVm>
{
    public int Skip { get; set; } = 1;
    public int Take { get; set; } = 10;
    public string Sort { get; set; } = string.Empty;
    public string? Filter { get; set; } = string.Empty;
}

public class GetUserGroupPaginationQueryHandler : IRequestHandler<GetUserGroupPaginationQuery, UserGroupVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetUserGroupPaginationQueryHandler(IMapper mapper, IApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<UserGroupVm> Handle(GetUserGroupPaginationQuery request, CancellationToken cancellationToken)
    {

        string[] sortParts = string.IsNullOrWhiteSpace(request.Sort) ? new[] { "Id", "desc" } : request.Sort.Split(' ');
        string sortKey = sortParts[0];
        string sortOrder = sortParts.Length > 1 ? sortParts[1] : "asc";

        IQueryable<UserGroup> queryableGroups = _context.UserGroups
        .Include(item => item.Members)
        .Where(item => item.Name.Contains(request.Filter ?? ""))
        .OrderByDescending(item => item.Id);

        if (sortKey == "name")
        {
            if (sortOrder == "asc")
                queryableGroups = queryableGroups.OrderBy(item => item.Name);
            else queryableGroups = queryableGroups.OrderByDescending(item => item.Name);
        }
        if (sortKey == "status")
        {
            if (sortOrder == "asc")
                queryableGroups = queryableGroups.OrderBy(item => item.IsActive);
            else queryableGroups = queryableGroups.OrderByDescending(item => item.IsActive);
        }

        var userGroups = await queryableGroups
        .ProjectTo<UserGroupDto>(_mapper.ConfigurationProvider)
        .PaginatedListGridAsync(request.Skip, request.Take);

        var userGroupVm = new UserGroupVm
        {
            PageNumber = userGroups.PageNumber,
            TotalPages = userGroups.TotalPages,
            TotalCount = userGroups.TotalCount,
            Items = new List<UserGroupItem>()
        };

        foreach (var group in userGroups.Items)
        {
            var userGroup = new UserGroupItem
            {
                Name = group.Name,
                IsActive = group.IsActive,
                Id = group.Id
            };

            userGroup.Members = new List<UserInfoDto>();

            foreach (var member in group.Members)
            {
                var user = _userService.GetUserById(member.UserId);
                if (user != null && user.Result != null)
                {
                    var groupMember = new UserInfoDto
                    {
                        FirstName = user.Result.FirstName,
                        LastName = user.Result.LastName,
                        UserId = member.UserId,
                        Id = member.UserId
                    };
                    userGroup.Members.Add(groupMember);
                }
            }


            userGroupVm.Items.Add(userGroup);
        }
        return userGroupVm;
    }
}

