using Asp.Versioning;

using FluentValidation;

using Marten;

using Npgsql;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using RecipeBook.Api.Infrastructure.Middleware;
using RecipeBook.Api.Infrastructure.OpenApi;

using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace RecipeBook.Api.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, logConfig) =>
        {
            logConfig.ReadFrom.Configuration(context.Configuration);
            logConfig.WriteTo.OpenTelemetry(
                endpoint: "http://localhost:4317",
                protocol: OtlpProtocol.Grpc,
                includedData: IncludedData.MessageTemplateTextAttribute
                              | IncludedData.MessageTemplateMD5HashAttribute
                              | IncludedData.TraceIdField
                              | IncludedData.SpanIdField
                              | IncludedData.SpecRequiredResourceAttributes
                              | IncludedData.SourceContextAttribute,
                resourceAttributes: new Dictionary<string, object>
                {
                    { "service.name", builder.Environment.ApplicationName },
                    { "service.instance.id", Environment.MachineName }
                });
        });

        return builder;
    }

    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi("v1", opts =>
        {
            opts.AddDocumentTransformer<RecipeBookDocumentTransformer>();
            opts.ShouldInclude = description => description.GroupName is "v1";
        });

        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: builder.Environment.ApplicationName,
                    autoGenerateServiceInstanceId: false,
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddNpgsql())
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddNpgsqlInstrumentation())
            .UseOtlpExporter();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("RecipeBook")
                               ?? throw new InvalidOperationException("Connection string not found");

        builder.Services.AddMarten(opts =>
            {
                opts.Connection(connectionString);
                opts.DatabaseSchemaName = "recipebook";
                opts.ApplicationAssembly = typeof(IRecipeBookApiMarker).Assembly;
            })
            .UseLightweightSessions();

        return builder;
    }

    public static WebApplicationBuilder AddApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("Api-Version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IRecipeBookApiMarker>();

        return builder;
    }
}