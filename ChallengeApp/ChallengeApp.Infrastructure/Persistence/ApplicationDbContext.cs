using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using ChallengeApp.Application.Common.Interfaces;
using ChallengeApp.Domain.Common;
using ChallengeApp.Domain.Entities;
using ChallengeApp.Infrastructure.Common;
using ChallengeApp.Infrastructure.Identity;
using ChallengeApp.Infrastructure.Persistence.Interceptors;

namespace ChallengeApp.Infrastructure.Persistence;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IDateTime _dateTime;
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IMediator mediator,
        IDateTime dateTime,
        ICurrentUserService currentUserService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _dateTime = dateTime;
        _currentUserService = currentUserService;
    }
    public DatabaseFacade DatabaseFacade => Database;
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<UserUserGroup> UserUserGroups => Set<UserUserGroup>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
                case EntityState.Deleted:
                    entry.Entity.DeletedBy = _currentUserService.UserId;
                    entry.Entity.Deleted = _dateTime.Now;
                    break;
            }
        }

        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}