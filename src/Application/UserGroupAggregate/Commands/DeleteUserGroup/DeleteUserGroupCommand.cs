using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Domain.Entities;
using MediatR;

namespace ChallengeApp.Application.UserGroupAggregate.Commands.DeleteUserGroup;

public record DeleteUserGroupCommand : IRequest<long>
{
    public long Id { get; set; }
}

public class DeleteUserGroupCommandHandler : IRequestHandler<DeleteUserGroupCommand, long>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.DatabaseFacade.BeginTransactionAsync(cancellationToken);
        try 
        {
            var entity = await _context.UserGroups
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(UserGroup), request.Id);
            }

            _context.UserGroups.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            return request.Id;
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(ex.Message);
        }
    }
}
