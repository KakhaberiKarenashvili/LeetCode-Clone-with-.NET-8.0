using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Pagination;

public class PagedList<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int TotalPages => (int) Math.Ceiling(TotalCount / (double) PageSize);
    public List<T> Items { get; set; }

    
    private PagedList(int pageNumber, int pageSize, int totalCount, List<T> items)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    

    /// <summary>
    /// Creates a paginated list from an IQueryable source using Entity Framework async operations.
    /// Use this method when working directly with database entities without DTO projection.
    /// </summary>
    /// <typeparam name="T">The entity type from the database (e.g., Problem, Submission, User)</typeparam>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="source">The IQueryable source to paginate (must be EF entities, not projected DTOs)</param>
    /// <returns>A PagedList containing the entities with pagination metadata</returns>
    /// <example>
    /// <code>
    /// var result = await PagedList&lt;Problem&gt;.CreateAsync(1, 10, _context.Problems);
    /// </code>
    /// </example>
    /// <remarks>
    /// This method performs two database queries: one for counting total records and one for fetching the page data.
    /// For DTO projection scenarios, use the generic CreateAsync&lt;TEntity, TResult&gt; overload instead.
    /// </remarks>

    public static async Task<PagedList<T>> CreateAsync(int pageNumber, int pageSize, IQueryable<T> source)
    {
        var totalCount = await source.CountAsync();
        var pagedItems = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        return new PagedList<T>(pageNumber, pageSize, totalCount, pagedItems);
    }


    /// <summary>
    /// Creates a paginated list by querying entities from the database and projecting them to DTOs.
    /// Uses Entity Framework async operations on entities, then applies projection in memory.
    /// </summary>
    /// <typeparam name="TEntity">The database entity type (e.g., Problem, Submission, User)</typeparam>
    /// <typeparam name="TResult">The result DTO type (e.g., ProblemDto, SubmissionDto)</typeparam>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="entityQuery">The IQueryable of database entities to paginate</param>
    /// <param name="selector">Function to transform entity to DTO (e.g., Problem => ProblemDto)</param>
    /// <returns>A PagedList containing the projected DTOs with pagination metadata</returns>
    /// <example>
    /// <code>
    /// var result = await PagedList&lt;ProblemDto&gt;.CreateAsync(
    ///     1, 10, 
    ///     _context.Problems, 
    ///     problem => new ProblemDto { Id = problem.Id, Name = problem.Name }
    /// );
    /// </code>
    /// </example>

    public static async Task<PagedList<TResult>> CreateAsync<TEntity,TResult>
        (int pageNumber, int pageSize, IQueryable<TEntity> entityQuery, Func<TEntity,TResult> selector)
    {
        var entities = await entityQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        var totalCount =await entityQuery.CountAsync();
        var pagedItems = entities.Select(selector).ToList();
        
        return new PagedList<TResult>(pageNumber, pageSize, totalCount, pagedItems);
    }
    
}