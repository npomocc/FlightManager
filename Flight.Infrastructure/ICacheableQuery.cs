using MediatR;

namespace Flight.Infrastructure;

public interface ICacheableQuery<TResponse> : IRequest<TResponse>
{
    bool BypassCache { get; }
    string CacheKey { get; }
    TimeSpan? SlidingExpiration { get; }
}