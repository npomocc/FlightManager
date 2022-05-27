using System.Linq.Expressions;
using Flight.Common;

namespace Flight.Data;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Get(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "");

    IQueryable<T> GetNoTracking(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "");
    PagedResult<T> Paging(
        int page, int pageSize,
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "");

    Task<T> GetById(object id, CancellationToken cancellationToken);

    Task Insert(T entity, CancellationToken cancellationToken);
    Task InsertMany(T[] entities);
    //Task Drop();
    Task Delete(object id, CancellationToken cancellationToken);

    Task Delete(T entityToDelete, CancellationToken cancellationToken);

    Task Update(T entityToUpdate, CancellationToken cancellationToken);
    Task<long> Total(CancellationToken cancellationToken);
}