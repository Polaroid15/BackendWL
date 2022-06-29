using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace WL.Host.Extensions; 

public static class SerilogExtensions {
    public static void UseSerilogProvider(this IHostBuilder hostBuilder, ElasticSettings? settings = null) {
        hostBuilder.UseSerilog((context, configuration) => {
            var indexFormat =
                $"{context.HostingEnvironment.ApplicationName}-logs-{context.HostingEnvironment.EnvironmentName}-{DateTime.UtcNow:dd-MM-yyyy}";
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
            if (settings != null) {
                configuration
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(settings.Url)) {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        ModifyConnectionSettings = config => config.BasicAuthentication(settings.Login, settings.Password),
                        IndexFormat = indexFormat,
                        InlineFields = true,
                    })
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithExceptionDetails();
            }
        });
    }
}