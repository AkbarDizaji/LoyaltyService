using StackExchange.Redis;

namespace LoyaltyService.Middlewares;

public class RequestCachingMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
{
    public async Task Invoke(HttpContext context)
    {

        var key = GetCacheKey(context);
        var cache = redis.GetDatabase();

        if (await cache.KeyExistsAsync(key))
        {
            var cachedResponse = await cache.StringGetAsync(key);
            await context.Response.WriteAsync(cachedResponse!);
            return;
        }

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseString = await new StreamReader(responseBody).ReadToEndAsync();

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);

        await cache.StringSetAsync(key, responseString, TimeSpan.FromSeconds(2));
    }

    private string GetCacheKey(HttpContext context)
    {
        var key = string.Empty;
        key += context.Request.Path;
        if (context.Request.QueryString.HasValue)
        {
            key += context.Request.QueryString.Value;
        }
        key += "_" + context.Request.Method;
        return key;
    }
}