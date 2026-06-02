using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;

namespace MainApp.Application.Extensions.Filtering;

public static class ProblemFilterExtensions
{
    public static IQueryable<Problem> FilterByName(this IQueryable<Problem> query, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return query;

        return query.Where(p => p.Name.Contains(name));
    }

    public static IQueryable<Problem> FilterByDifficulty(this IQueryable<Problem> query, Difficulty? difficulty)
    {
        if (difficulty == null)
            return query;

        return query.Where(p => p.Difficulty == difficulty);
    }

    public static IQueryable<Problem> FilterByCategories(this IQueryable<Problem> query, List<Category>? categories)
    {
        if (categories == null || categories.Count == 0)
            return query;

        return query.ApplyCategoryPriorityFilter(categories);
    }

    public static IQueryable<Problem> ApplyCategoryPriorityFilter(this IQueryable<Problem> query, List<Category> categories)
    {
        return query
            .Where(p => p.Categories.Any(c => categories.Contains(c)))
            .OrderBy(p => categories.All(c => p.Categories.Contains(c)) ? 1 : 2)
            .ThenBy(p => p.Name);
    }

    public static IQueryable<Problem> ApplyFilter(this IQueryable<Problem> query,
        string? name, Difficulty? difficulty, List<Category>? categories)
    {
        return query
            .FilterByName(name)
            .FilterByDifficulty(difficulty)
            .FilterByCategories(categories);
    }
}
