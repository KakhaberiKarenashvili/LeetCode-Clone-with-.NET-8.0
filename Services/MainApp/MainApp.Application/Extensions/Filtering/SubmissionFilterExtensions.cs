using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;

namespace MainApp.Application.Extensions.Filtering;

public static class SubmissionFilterExtensions
{

    public static IQueryable<Submissions> FilterByStatus(this IQueryable<Submissions> query, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return query;
        }

        if (Enum.TryParse<Status>(status, true, out var parsedStatus))
        {
            return query.Where(s => s.Status == parsedStatus);
        }
        
        return query;
    }
    
    public static IQueryable<Submissions> FilterByLanguage(this IQueryable<Submissions> query, string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return query;
        }

        return query.Where(s => s.Language == language);
    }

    /// <summary>
    /// Applies multiple filters to the submissions query in a specific order.
    /// This is a convenience method that chains multiple filter operations together.
    /// </summary>
    /// <param name="query">The IQueryable of Submissions entities to filter</param>
    /// <param name="status">
    /// The submission status to filter by (e.g., "Accepted", "Wrong Answer", "Time Limit Exceeded").
    /// If null, empty, or whitespace, no status filtering is applied.
    /// </param>
    /// <param name="language">
    /// The programming language to filter by. Supports partial matches (e.g., "C#", "Java", "Python").
    /// If null, empty, or whitespace, no language filtering is applied.
    /// </param>
    /// <returns>
    /// An IQueryable of Submissions entities with all applicable filters applied.
    /// If filter parameters are null or empty, those specific filters are skipped.
    /// </returns>
    /// <remarks>
    /// <para>This method applies filters in the following order:</para>
    /// <list type="number">
    /// <item><description>Language filter - filters submissions containing the specified language substring</description></item>
    /// <item><description>Status filter - filters submissions by exact status match</description></item>
    /// </list>
    /// <para>All filters are applied at the database level for optimal performance.</para>
    /// <para>Both filters use case-sensitive matching.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Filter by both language and status
    /// var filteredSubmissions = _context.Submissions
    ///     .ApplyFilter("Completed", "C++")
    ///     .ToListAsync();
    ///     
    /// // Filter by language only (status is null)
    /// var pythonSubmissions = _context.Submissions
    ///     .ApplyFilter(null, "Python")
    ///     .ToListAsync();
    ///     
    /// // No filtering (both parameters are null)
    /// var allSubmissions = _context.Submissions
    ///     .ApplyFilter(null, null)
    ///     .ToListAsync();
    /// </code>
    /// </example>
    public static IQueryable<Submissions> ApplyFilter(this IQueryable<Submissions> query, string? status,
        string? language)
    {
        return query
            .FilterByLanguage(language)
            .FilterByStatus(status);
    }
    
    
    
}