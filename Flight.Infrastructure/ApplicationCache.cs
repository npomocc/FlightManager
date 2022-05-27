using Flight.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Flight.Infrastructure;

public class ApplicationCache : IApplicationCache
{
    private const string ThisName = nameof(ApplicationCache);
    private readonly IDistributedCache _cache;
    private readonly ILogger _logger;
    private readonly TimeSpan _slidingExpiration;

    public ApplicationCache(IDistributedCache cache,
        IConfiguration config,
        ILogger<ApplicationCache> logger)
    {
        _cache = cache;
        _logger = logger;
        var slidingExpiration = config["CacheSlidingExpiration"].ToInt();
        _slidingExpiration = TimeSpan.FromMinutes(
            slidingExpiration <= 0
                ? 15
                : slidingExpiration);
    }

    public async Task<T?> GetFromCache<T>(string key, CancellationToken cancellationToken)
    {
        var logHeaderId = $"{ThisName}/{nameof(GetFromCache)}: ";
        _logger.LogInformation($"{logHeaderId}fetching: '{key}' from cache.");
        var cachedObj = await _cache.GetAsync(key, cancellationToken);
        if (cachedObj is null)
        {
            _logger.LogWarning($"{logHeaderId}key: '{key}' not found.");
            return default;
        }
        var cachedStr = cachedObj.GetSafeString(Encoding.Unicode);
        if (string.IsNullOrWhiteSpace(cachedStr))
        {
            _logger.LogWarning($"{logHeaderId}remove empty cache for key: '{key}'.");
            await _cache.RemoveAsync(key, cancellationToken);
            return default;
        }
        _logger.LogInformation($"fetched from cache -> '{key}'.");
        return cachedStr.SafeDeserialize<T>();
    }

    public async Task SetCacheAsync<T>(string key, T item, CancellationToken cancellationToken)
    {
        var logHeaderId = $"{ThisName}/{nameof(SetCacheAsync)}: ";
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning($"{logHeaderId}empty cache key"); 
                await Task.FromException(new NullReferenceException("empty_cache_key"));
            }
            if (item is null)
            {
                _logger.LogWarning($"{logHeaderId}empty cache item");
                await Task.FromException(new NullReferenceException("empty_cache_item"));
            }
            var options = new DistributedCacheEntryOptions { SlidingExpiration = _slidingExpiration };
            var str = item.SafeSerialize();
            if (string.IsNullOrWhiteSpace(str))
            {
                _logger.LogWarning($"{logHeaderId}empty serialized cache string");
                await Task.FromException(new NullReferenceException("empty_cache_string"));
            }

            var serializedData = str.GetSafeBytes(Encoding.Unicode);
            if (serializedData == Array.Empty<byte>())
            {
                _logger.LogWarning($"{logHeaderId}empty cache data bytes");
                await Task.FromException(new NullReferenceException("empty_cache_data"));
            }
            await _cache.SetAsync(key, serializedData, options, cancellationToken);
            _logger.LogInformation($"{logHeaderId}added to cache -> '{key}'.");
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"{logHeaderId}failed: {exception.Trace()}");
            await Task.FromException(exception);
        }
    }

    public async Task<T> GetOrSetCache<T>(string key, T view, CancellationToken cancellationToken)
    {
        var logHeaderId = $"{ThisName}/{nameof(GetOrSetCache)}: ";
        _logger.LogInformation($"{logHeaderId}get key: '{key}' from cache.");
        var cachedResponse = await _cache.GetAsync(key, cancellationToken);
        if (cachedResponse is null)
        {
            _logger.LogWarning($"{logHeaderId}key: '{key}' not found, set new.");
            await SetCacheAsync(key, view, cancellationToken);
            return view;
        }
        var cached = cachedResponse.GetSafeString(Encoding.Unicode);
        if (string.IsNullOrWhiteSpace(cached))
        {
            _logger.LogWarning($"{logHeaderId}empty cache for key: '{key}'.");
            await SetCacheAsync(key, view, cancellationToken);
            return view;
        }
        var cview = cached.SafeDeserialize<T>();
        if (cview is null)
        {
            _logger.LogWarning($"{logHeaderId}item for key: '{key}' is null.");
            await SetCacheAsync(key, view, cancellationToken);
            return view;
        }
        _logger.LogInformation($"{logHeaderId}fetched from cache -> '{key}'.");
        return view;
    }

    public async Task<bool> Remove(string key, CancellationToken cancellationToken)
    {
        var logHeaderId = $"{ThisName}/{nameof(Remove)}: ";
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"{logHeaderId}failed: {exception.Trace()}");
            return false;
        }
    }
}