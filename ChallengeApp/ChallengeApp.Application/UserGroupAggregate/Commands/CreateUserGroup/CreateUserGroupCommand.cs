using System.Security.Cryptography;
using ChallengeApp.Application.Common.Exceptions;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Domain.Entities;
using MediatR;

namespace ChallengeApp.Application.UserGroupAggregate.Commands.CreateUserGroup;

public record CreateUserGroupCommand : IRequest<long>
{
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


public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateUserGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.DatabaseFacade.BeginTransactionAsync(cancellationToken);
        try 
        {
            var entity = new UserGroup
            {
                Name = request.Name,
                IsActive = true
            };

            // Duplicate not allowed
            var existName = _context.UserGroups.Where(item => item.Name.ToUpper() == request.Name.ToUpper()).SingleOrDefault();
            if (existName != null) 
            {
                 throw new AlreadyExistsException(nameof(UserGroup), request.Name);
            }
                
            _context.UserGroups.Add(entity);
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

    public async Task AddUserGroupMembers(List<Member> userGroupMember, long userId, CancellationToken cancellationToken)
    {
        var insertedUsers = new List<UserUserGroup>();
        
        foreach(var member in userGroupMember)
        {
            insertedUsers.Add(new UserUserGroup
            {
                UserGroupId = userId,
                UserId = member.Id
            });
        }
        await _context.UserUserGroups.AddRangeAsync(insertedUsers);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
}
