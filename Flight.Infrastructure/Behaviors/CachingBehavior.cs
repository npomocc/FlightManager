using Flight.Common;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Flight.Infrastructure.Behaviors
{
    /// <summary>
    /// Поведение кэширования для команд и запросов CQRS
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICacheableQuery<TResponse>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        private readonly TimeSpan _slidingExpiration;
        public CachingBehavior(IDistributedCache cache,
            IConfiguration config, 
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
            var slidingExpiration = config["CacheSlidingExpiration"].ToInt();
            _slidingExpiration = TimeSpan.FromMinutes(
                slidingExpiration <= 0
                    ? 15
                    : slidingExpiration);

        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response;
            if (request.BypassCache) return await next();
            async Task<TResponse> GetResponseAndAddToCache()
            {
                response = await next();
                var slidingExpiration = _slidingExpiration;
                var options = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };
                var str = response.SafeSerialize();
                if (string.IsNullOrWhiteSpace(str))
                    return response;
                var serializedData = str.GetSafeBytes(Encoding.Unicode);
                await _cache.SetAsync(request.CacheKey, serializedData, options, cancellationToken);
                return response;
            }
            var cachedResponse = await _cache.GetAsync(request.CacheKey, cancellationToken);
            if (cachedResponse is not null)
            {
                var cached = cachedResponse.GetSafeString(Encoding.Unicode);
                if (string.IsNullOrWhiteSpace(cached))
                {
                    response = await GetResponseAndAddToCache();
                    _logger.LogInformation($"added to cache -> '{request.CacheKey}'.");
                }
                else
                {
                    response = cached.SafeDeserialize<TResponse>();
                    _logger.LogInformation($"fetched from cache -> '{request.CacheKey}'.");
                }
            }
            else
            {
                response = await GetResponseAndAddToCache();
                _logger.LogInformation($"added to cache -> '{request.CacheKey}'.");
            }
            return response;
        }
    }
}
