namespace BuildingBlocks.Common.Pagination;

public class PagedList<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Items { get; set; }
    
    public bool HasPreviousPage => PageNumber > 1;
    
    public bool HasNextPage => PageNumber < TotalPages;
    private int TotalPages => (int) Math.Ceiling(TotalCount / (double) PageSize);
    
    private PagedList(int pageNumber, int pageSize, int totalCount, List<T> items)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static async Task<PagedList<T>> CreateAsync(int pageNumber, int pageSize, IQueryable<T> source)
    {
        var pagedItems = source.Skip((pageNumber - 1) * pageNumber).Take(pageSize).ToList();
        
        var totalCount = source.Count();
        
        return new PagedList<T>(pageNumber, pageSize, totalCount, pagedItems);
    }
    
}