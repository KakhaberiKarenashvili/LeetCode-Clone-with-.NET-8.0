using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;

namespace MainApp.Application.Extensions.Filtering;

public static class ProblemFilterExtensions
{
    public static IQueryable<Problem> FilterByName(this IQueryable<Problem> query, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return query;
        }

        return query.Where(p => p.Name.Contains(name));
    }

    public static IQueryable<Problem> FilterByDifficulty(this IQueryable<Problem> query, string? difficulty)
    {
        if (string.IsNullOrWhiteSpace(difficulty))
        {
            return query;
        }

        if (Enum.TryParse<Difficulty>(difficulty, out var parsedDifficulty))
        {
            return query.Where(p => p.Difficulty == parsedDifficulty);
        }

        return query;
    }

    public static IQueryable<Problem> FilterByCategories(this IQueryable<Problem> query, List<string>? categories)
    {
        if (categories == null || categories.Count == 0)
        {
            return query;
        }
        
        var categoryFlags = new List<Category>();
        
        foreach (var category in categories)
        {
            if (Enum.TryParse<Category>(category, true, out var categoryEnum))
            {
                categoryFlags.Add(categoryEnum);
            }
        }

        if (!categoryFlags.Any())
            return query;

        return query.ApplyCategoryPriorityFilter(categoryFlags);
        
    }


    public static IQueryable<Problem> ApplyCategoryPriorityFilter(this IQueryable<Problem> query,
        List<Category> categoryFlags)
    {
        //Flags
        var exactFlag = categoryFlags.Aggregate((current, next) => current | next);
        
        //First priority exact matches
        var exactMatches = query.Where(p => p.Category == exactFlag);
        
        //Second priority contains all requested categories (but might have more)
        var containsAllMatches = query.Where(p => 
            categoryFlags.All(flag => (p.Category & flag) == flag) && 
            p.Category != exactFlag);

        return exactMatches
            .Union(containsAllMatches)
            .Distinct();
    }

    public static IQueryable<Problem> ApplyFilter(this IQueryable<Problem> query,
        string? name, string? difficulty, List<string>? categories)
    {
        return query
            .FilterByName(name)
            .FilterByDifficulty(difficulty)
            .FilterByCategories(categories);
    }
    
}