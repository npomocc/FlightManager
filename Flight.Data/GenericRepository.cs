using System.Linq.Expressions;
using Flight.Common;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data;

/// <summary>
/// Абстрактный репозиторий
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;
    protected DbContext Context { get; }

    protected GenericRepository(DbContext context)
    {
        Context = context;
        _dbSet = context.Set<T>();
    }
    /// <summary>
    /// Запрос с параметрами
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="includeProperties"></param>
    /// <returns></returns>
    public virtual IQueryable<T> Get(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;
        if (filter is not null)
            query = query.Where(filter);
        foreach (var includeProperty in includeProperties.Split
                     (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(includeProperty);

        return orderBy is not null
            ? orderBy(query)
            : query;
    }

    public virtual IQueryable<T> GetNoTracking(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;
        if (filter is not null)
            query = query.Where(filter);
        query = includeProperties
            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return orderBy is not null
            ? orderBy(query.AsNoTracking())
            : query.AsNoTracking();
    }

    public virtual PagedResult<T> Paging(
        int page, int pageSize,
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;
        if (filter is not null)
            query = query.Where(filter);
        query = includeProperties
            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return orderBy is not null
            ? orderBy(query).GetPaged(page, pageSize)
            : query.GetPaged(page, pageSize);
    }

    public virtual async Task<T> GetById(object id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task Insert(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Add(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
    public Task InsertMany(T[] entities)
    {
        throw new NotImplementedException("InsertMany");
    }
    public Task Drop()
    {
        throw new NotImplementedException("Delete");
    }
    public virtual async Task Delete(object id, CancellationToken cancellationToken)
    {
        var entityToDelete = await _dbSet.FindAsync(new[] { id }, cancellationToken: cancellationToken);
        if (entityToDelete != null) await Delete(entityToDelete, cancellationToken);
    }
    public virtual async Task Delete(T entityToDelete, CancellationToken cancellationToken)
    {
        if (Context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbSet.Remove(entityToDelete);
        await Context.SaveChangesAsync(cancellationToken);
    }
    public virtual async Task Update(T entityToUpdate, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entityToUpdate);
        Context.Entry(entityToUpdate).State = EntityState.Modified;
        await Context.SaveChangesAsync(cancellationToken);
    }
    public async Task<long> Total(CancellationToken cancellationToken)
    {
        return await Context.Set<T>().CountAsync(cancellationToken: cancellationToken);
    }
}