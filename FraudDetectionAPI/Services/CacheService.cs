using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FraudDetectionAPI.Services
{
    /// <summary>
    /// Redis caching service for improved performance
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }

    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache? _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly bool _isEnabled;
        private readonly DistributedCacheEntryOptions _defaultOptions;

        public RedisCacheService(
            IDistributedCache? cache, 
            IConfiguration configuration,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _isEnabled = configuration.GetValue<bool>("Redis:Enabled", false);

            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            if (_isEnabled)
            {
                _logger.LogInformation("✅ Redis cache service enabled");
            }
            else
            {
                _logger.LogInformation("ℹ️ Redis cache disabled - running without caching");
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (!_isEnabled || _cache == null)
                return default;

            try
            {
                var json = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(json))
                    return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Cache get failed for key {Key}: {Error}", key, ex.Message);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (!_isEnabled || _cache == null)
                return;

            try
            {
                var json = JsonSerializer.Serialize(value);
                var options = expiration.HasValue
                    ? new DistributedCacheEntryOptions 
                      { 
                          AbsoluteExpirationRelativeToNow = expiration 
                      }
                    : _defaultOptions;

                await _cache.SetStringAsync(key, json, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Cache set failed for key {Key}: {Error}", key, ex.Message);
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (!_isEnabled || _cache == null)
                return;

            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Cache remove failed for key {Key}: {Error}", key, ex.Message);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (!_isEnabled || _cache == null)
                return false;

            try
            {
                var value = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Cache exists check failed for key {Key}: {Error}", key, ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// In-memory cache fallback when Redis is not available
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = new();
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(ILogger<InMemoryCacheService> logger)
        {
            _logger = logger;
            _logger.LogInformation("ℹ️ Using in-memory cache (Redis fallback)");
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.Expiration > DateTime.UtcNow)
                {
                    return Task.FromResult((T?)entry.Value);
                }
                _cache.Remove(key);
            }
            return Task.FromResult(default(T?));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var exp = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(30));
            _cache[key] = (value!, exp);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.ContainsKey(key) && 
                                   _cache[key].Expiration > DateTime.UtcNow);
        }
    }
}
