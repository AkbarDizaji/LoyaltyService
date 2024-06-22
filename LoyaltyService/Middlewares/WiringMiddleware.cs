using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LoyaltyService.Middlewares;

public class WiringMiddleware(RequestDelegate next, LoyaltyContext dbContext)
{
    public async Task InvokeAsync(HttpContext context)
    {

        if ((await dbContext.Database.GetPendingMigrationsAsync(CancellationToken.None)).Any())
        {
            await dbContext.Database.MigrateAsync(CancellationToken.None);
        }
        await next(context);
    }
}