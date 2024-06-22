using Infrastructure;
using Serilog;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyService.Middlewares;

public class RequestResponseLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        
        // Log the request
        Log.Information("Request: {Method} {Path} {Body}",
            context.Request.Method,
            context.Request.Path,
            await GetRequestBody(context.Request));

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            // Call the next middleware in the pipeline
            await next(context);

            // Log the response
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            Log.Information("Response: {StatusCode} {Body}",
                context.Response.StatusCode,
                responseBody);
        }
        finally
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task<string> GetRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }
}