using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prometheus.HttpMetrics
{
    public sealed class HttpRequestDurationMiddleware : HttpRequestMiddlewareBase<Histogram>
    {
        private readonly RequestDelegate _next;
        private readonly Histogram _requestDuration;

        public HttpRequestDurationMiddleware(RequestDelegate next, Histogram histogram)
            : base(histogram)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _requestDuration = histogram;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopWatch.Stop();

                _requestDuration
                    .WithLabels(GetLabelData(context))
                    .Observe(stopWatch.Elapsed.TotalSeconds);
            }
        }
    }
}