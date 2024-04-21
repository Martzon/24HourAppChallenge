using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Application.Common.Models;
using MediatR;

namespace ChallengeApp.Application.UserAggregate.Commands.DeleteUser;

public class DeleteUser : IRequest
{
    public string Id { get; set; }
}


public class DeleteUserHandler : IRequestHandler<DeleteUser>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserService _userService;
    public DeleteUserHandler(IApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Unit> Handle(DeleteUser request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserById(request.Id);

        await _userService.DeleteUserAsync(user);
        return Unit.Value;
    }
}
