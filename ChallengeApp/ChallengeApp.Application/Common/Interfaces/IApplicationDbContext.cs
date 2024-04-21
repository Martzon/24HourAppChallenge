using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserGroup> UserGroups { get; }
    DbSet<UserUserGroup> UserUserGroups { get; }
    DbSet<AuditEvent> AuditEvents { get; }
    DatabaseFacade DatabaseFacade { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}