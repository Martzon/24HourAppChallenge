using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChallengeApp.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();

    public static Task<PaginatedList<TDestination>> PaginatedListGridAsync<TDestination>(this IQueryable<TDestination> queryable, int skip, int take) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), skip, take, true);
}
