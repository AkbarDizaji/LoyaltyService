using System.Text.Json;
using Application.Commons;
using Serilog;
using ILogger = Serilog.ILogger;

namespace LoyaltyService.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = Result<object>.Failure("An internal server error occurred.");

        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}