namespace Flight.Infrastructure;

public interface IApplicationCache
{
    Task<T?> GetFromCache<T>(string key, CancellationToken cancellationToken);
    Task SetCacheAsync<T>(string key, T item, CancellationToken cancellationToken);
    Task<T> GetOrSetCache<T>(string key, T view, CancellationToken cancellationToken);
    Task<bool> Remove(string key, CancellationToken cancellationToken);
}