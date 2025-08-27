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

        return query.Where(p => categoryFlags.Any(flag => (p.Category & flag) == flag)
            )
            .OrderBy(p => 
                p.Category == exactFlag ? 1 :
                categoryFlags.All(flag => (p.Category & flag) == flag) ? 2 :
                3)
            .ThenBy(p => p.Name);
    }
    
    /// <summary>
    /// Applies all problem filters from the provided filter DTO to the query.
    /// This is a convenience method that chains multiple filter operations together.
    /// </summary>
    /// <param name="query">The IQueryable of Problem entities to filter</param>
    /// <param name="filter">The filter DTO containing all filter criteria</param>
    /// <returns>
    /// An IQueryable of Problem entities with all applicable filters applied.
    /// If filter properties are null or empty, those specific filters are skipped.
    /// </returns>
    /// <remarks>
    /// <para>This method applies filters in the following order:</para>
    /// <list type="number">
    /// <item><description>Name filter - filters problems containing the specified name substring</description></item>
    /// <item><description>Difficulty filter - filters problems by exact difficulty match</description></item>
    /// <item><description>Category filter - filters problems by categories with priority-based matching:
    /// <list type="bullet">
    /// <item><description>Priority 1: Exact category match</description></item>
    /// <item><description>Priority 2: Contains all requested categories (may have more)</description></item>
    /// </list>
    /// </description></item>
    /// </list>
    /// <para>All filters are applied at the database level for optimal performance.</para>
    /// </remarks>
    /// <example>
    /// <code>
    ///     Name = "Two Sum", 
    ///     Categories = new List&lt;string&gt; { "Arrays", "Strings" },
    ///     Difficulty = "Easy"
    /// 
    /// var filteredProblems = _context.Problems
    ///     .ApplyProblemsFilter(Name, Categories, Difficulty)
    ///     .ToListAsync();
    /// </code>
    /// </example>

    public static IQueryable<Problem> ApplyFilter(this IQueryable<Problem> query,
        string? name, string? difficulty, List<string>? categories)
    {
        return query
            .FilterByName(name)
            .FilterByDifficulty(difficulty)
            .FilterByCategories(categories);
    }
    
}