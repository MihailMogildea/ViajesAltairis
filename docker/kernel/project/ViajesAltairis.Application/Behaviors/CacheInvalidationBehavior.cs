using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IInvalidatesCache
{
    private readonly ICacheService _cacheService;

    public CacheInvalidationBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        foreach (var prefix in TRequest.CachePrefixes)
            await _cacheService.RemoveByPrefixAsync(prefix, cancellationToken);

        return response;
    }
}
