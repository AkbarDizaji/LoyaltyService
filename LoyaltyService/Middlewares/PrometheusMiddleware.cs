using Prometheus;
using System.Diagnostics;

namespace LoyaltyService.Middlewares;

public class PrometheusMiddleware(RequestDelegate next)
{
    private readonly Counter _requestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests", new CounterConfiguration
    {
        LabelNames = new[] { "method", "endpoint", "status" }
    });
    private readonly Histogram _requestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds", new HistogramConfiguration
    {
        LabelNames = new[] { "method", "endpoint", "status" },
        Buckets = Histogram.ExponentialBuckets(0.001, 2, 10) 
    });

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        var method = context.Request.Method;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode.ToString();

            if (path != null)
            {
                _requestCounter.Labels(method, path, statusCode).Inc();
                _requestDuration.Labels(method, path, statusCode).Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
