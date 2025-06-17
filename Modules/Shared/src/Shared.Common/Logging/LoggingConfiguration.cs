using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Shared.Common.Logging
{
    public static class LoggingConfiguration
    {
        public static void ConfigureSerilog(WebApplicationBuilder builder, bool useSeq = false, bool useOtel = false)
        {
            var configuration = builder.Configuration;

            // Initialize LoggerConfiguration
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            // Configure Seq sink
            ConfigureSeqLogging(useSeq, configuration, logger);

            // Configure OpenTelemetry sink
            ConfigureOtelLogging(useOtel, configuration, logger);

            // Create and set the global logger
            Log.Logger = logger.CreateLogger();

            // Use Serilog as the logging provider
            builder.Host.UseSerilog(); ;
        }

        private static void ConfigureOtelLogging(bool useOtel, ConfigurationManager configuration, LoggerConfiguration logger)
        {
            if (useOtel)
            {
                var otelEndpoint = configuration["Logging:Otel:Endpoint"];
                var serviceName = configuration["Logging:Otel:ServiceName"] ?? "DefaultService";

                if (!string.IsNullOrEmpty(otelEndpoint))
                {
                    logger.WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = otelEndpoint; // OpenTelemetry OTLP/gRPC endpoint
                        options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc; // Use gRPC protocol
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            { "service.name", serviceName }
                        };
                    });
                }
                else
                {
                    Console.WriteLine("OpenTelemetry logging is enabled, but no endpoint is configured in 'Logging:Otel:Endpoint'");
                }
            }
        }

        private static void ConfigureSeqLogging(bool useSeq, IConfiguration configuration, LoggerConfiguration logger)
        {
            
            if (useSeq)
            {
                var seqEndpoint = configuration["Logging:Seq:Endpoint"];
                if (!string.IsNullOrEmpty(seqEndpoint))
                {
                    logger.WriteTo.Seq(seqEndpoint);
                }
                else
                {
                    Console.WriteLine("Seq logging is enabled, but no endpoint is configured in 'Logging:Seq:Endpoint'");
                }
                
            }
        }
    }
}
