using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly ConcurrentDictionary<string, ClientRateLimitInfo> _rateLimitStore = new();

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdFromContext(context);
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("Missing client ID");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing client ID");
            return;
        }

        var (isAllowed, retryAfter) = await IsClientRateLimitedAsync(clientId);
        if (!isAllowed)
        {
            _logger.LogWarning("Client {ClientId} is rate limited. Retry after {RetryAfter}", clientId, retryAfter);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = retryAfter?.TotalSeconds.ToString();
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        await _next(context);
    }

    private async Task<(bool IsAllowed, TimeSpan? RetryAfter)> IsClientRateLimitedAsync(string clientId)
    {
        var now = DateTime.UtcNow;
        var rateLimitInfo = _rateLimitStore.GetOrAdd(clientId, new ClientRateLimitInfo
        {
            LastRequestTime = now,
            RequestCount = 1
        });

        lock (rateLimitInfo)
        {
            if ((now - rateLimitInfo.LastRequestTime) > TimeSpan.FromMinutes(1))
            {
                rateLimitInfo.RequestCount = 1;
                rateLimitInfo.LastRequestTime = now;
                return (true, null);
            }

            if (rateLimitInfo.RequestCount < 100)
            {
                rateLimitInfo.RequestCount++;
                return (true, null);
            }

            var retryAfter = TimeSpan.FromMinutes(1) - (now - rateLimitInfo.LastRequestTime);
            return (false, retryAfter);
        }
    }

    private string? GetClientIdFromContext(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Client-ID", out var clientId))
        {
            return clientId.FirstOrDefault();
        }
        var claim = context.User?.FindFirst(c => c.Type == "PartnerId" || c.Type == "client_id");
            return claim?.Value;
    }
}

public class ClientRateLimitInfo
{
    public DateTime LastRequestTime { get; set; }
    public int RequestCount { get; set; }
}
