using MassTransit.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Infrastructure;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddCustomOpenTelemetry(this WebApplicationBuilder builder, string serviceName)
    {
        var OtlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.AddOtlpExporter(opt =>
            {
                if (!string.IsNullOrWhiteSpace(OtlpEndpoint))
                {
                    opt.Endpoint = new Uri(OtlpEndpoint);
                }
            });
        });

        var otel = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName));

        otel.WithMetrics(metrics =>
        {
            // Metrics provider from OpenTelemetry
            metrics.AddAspNetCoreInstrumentation();
            // Metrics provides by ASP.NET Core in .NET 8
            metrics.AddMeter("Microsoft.AspNetCore.Hosting");
            metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

            metrics.AddOtlpExporter(opt =>
            {
                if (!string.IsNullOrWhiteSpace(OtlpEndpoint))
                {
                    opt.Endpoint = new Uri(OtlpEndpoint);
                }
            });
        });

        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(DiagnosticHeaders.DefaultListenerName);
            tracing.AddOtlpExporter(opt =>
            {
                if (!string.IsNullOrWhiteSpace(OtlpEndpoint))
                {
                    opt.Endpoint = new Uri(OtlpEndpoint);
                }
            });
        });

        return builder;
    }
}