using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MyEducation.MyIdentity.Interface;
using MyEducation.MyIdentity.Models.Auth;

public class ClientSourceCacheService : IClientSourceCacheService
{
    private readonly IClientSourceRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ClientSourceCacheService> _logger;

    private const string CacheKeyPrefix = "ClientSource_";

    public ClientSourceCacheService(IClientSourceRepository repository, IMemoryCache cache, ILogger<ClientSourceCacheService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ClientSource?> GetClientAsync(string clientId)
    {
        var cacheKey = CacheKeyPrefix + clientId;
        if (_cache.TryGetValue(cacheKey, out ClientSource? cachedClient))
        {
            _logger.LogInformation("Client {ClientId} found in cache", clientId);
            return cachedClient;
        }

        var client = await _repository.GetByIdAsync(clientId);
        if (client != null)
        {
            _cache.Set(cacheKey, client, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
            _logger.LogInformation("Client {ClientId} added to cache", clientId);
        }
        else
        {
            _logger.LogWarning("Client {ClientId} not found in repository", clientId);
        }

        return client;
    }

    public async Task InvalidateCacheAsync(string clientId)
    {
        var cacheKey = CacheKeyPrefix + clientId;
        _cache.Remove(cacheKey);
        _logger.LogInformation("Cache invalidated for client {ClientId}", clientId);
    }

    public async Task<bool> IsClientValidAsync(string clientId)
    {
        var client = await GetClientAsync(clientId);
        return client?.IsValid() ?? false;
    }
}