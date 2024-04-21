using AutoMapper;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using MediatR;

namespace ChallengeApp.Application.UserAggregate.Queries;

public class GetUsers : IRequest<List<UserAccount>>
{
    public string? Keyword { get; set; }
}

public class GetUsersHandler : IRequestHandler<GetUsers, List<UserAccount>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetUsersHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<List<UserAccount>> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        var userList = await _userService.GetUserByKeyword(request.Keyword);

        return userList;
    }
}


