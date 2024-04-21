using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Application.UserGroupAggregate.Commands.UpdateUserGroup;

public record UpdateUserGroupCommand : IRequest<long>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<UserGroupPermission> UserGroupModulePermissions { get; set; }
    public List<Member> Members { get; set; }
}

public record UserGroupPermission
{
    public long ModuleId { get; set; }
    public long PermissionId { get; set; }
}

public record Member
{
    public string Id { get; set; }
    public string Name { get; set; }
}


public class CreateUserGroupCommandHandler : IRequestHandler<UpdateUserGroupCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateUserGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.DatabaseFacade.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = await _context.UserGroups
            .Where(l => l.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

            // Not found
            if (entity == null)
            {
                throw new NotFoundException(nameof(UserGroup), request.Id);
            }

            // Duplicate not allowed
            var existName = _context.UserGroups.Where(item =>
                item.Name.ToUpper() == request.Name.ToUpper() &&
                item.Id != request.Id)
                .FirstOrDefault();

            if (existName != null)
            {
                throw new AlreadyExistsException(nameof(UserGroup), request.Name);
            }

            entity.Name = request.Name;
            _context.UserGroups.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.Members != null)
            {
                await AddUserGroupMembers(request.Members, entity.Id, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return entity.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(ex.Message);
        }

    }

    public async Task AddUserGroupMembers(List<Member> userGroupMember, long userGroupId, CancellationToken cancellationToken)
    {
        var insertedUsers = new List<UserUserGroup>();

        var currentMembers = await _context.UserUserGroups.Where(e => e.UserGroupId == userGroupId).ToListAsync();
        var deletedMembers = currentMembers.Where(e => !userGroupMember.Select(gm => gm.Id).ToList().Contains(e.UserId)).ToList();

        foreach (var member in userGroupMember)
        {
            var entity = currentMembers.Where(item => item.UserId == member.Id).FirstOrDefault();

            if (entity == null)
            {
                var newUser = new UserUserGroup
                {
                    UserGroupId = userGroupId,
                    UserId = member.Id
                };
                _context.UserUserGroups.Add(newUser);
            }
        }


        foreach (var deleted in deletedMembers)
        {
            _context.UserUserGroups.Remove(deleted);
        }


        await _context.UserUserGroups.AddRangeAsync(insertedUsers);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
