using AutoMapper;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using MediatR;

namespace ChallengeApp.Application.UserAggregate.Queries;

public class GetUsersWithPagination : IRequest<PaginatedList<UserAccountDto>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public string Sort { get; set; } = string.Empty;
    public string? Filter { get; set; } = string.Empty;
}

public class GetUsersWithPaginationHandler : IRequestHandler<GetUsersWithPagination, PaginatedList<UserAccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetUsersWithPaginationHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<PaginatedList<UserAccountDto>> Handle(GetUsersWithPagination request, CancellationToken cancellationToken)
    {
        var userList = await _userService.GetUserWithPagination(request.Filter, request.Take, request.Skip, request.Sort);
        return userList;
        
    }
}


