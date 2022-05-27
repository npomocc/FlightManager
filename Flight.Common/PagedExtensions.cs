namespace Flight.Common;

public static class PagedExtensions
{
    public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
        int page, int pageSize) where T : class?
    {
        if (page == 0) page = 1;
        if (pageSize == 0) pageSize = 20;
        var result = new PagedResult<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = query.Count()
        };


        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = query.Skip(skip).Take(pageSize).ToList();

        return result;
    }
    public static PagedResult<T> GetPaged<T>(this IEnumerable<T> query,
        int page, int pageSize) where T : class
    {
        if (page == 0) page = 1;
        if (pageSize == 0) pageSize = 20;
        var list = query.ToList();
        var result = new PagedResult<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = list.Count
        };


        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = list.Skip(skip).Take(pageSize).ToList();

        return result;
    }

}