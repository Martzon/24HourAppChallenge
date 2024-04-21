using AutoMapper;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using MediatR;

namespace ChallengeApp.Application.UserAggregate.Queries;

public class GetUserByEmail : IRequest<UserAccount>
{
    public string Email { get; set; }
}

public class GetUserByEmailsHandler : IRequestHandler<GetUserByEmail, UserAccount>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetUserByEmailsHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<UserAccount> Handle(GetUserByEmail request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetUserByEmail(request.Email);            
            user.UserRoles = await _userService.GetRolesByEmail(user.Email);
            
            return user;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}


