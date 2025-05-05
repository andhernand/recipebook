using Asp.Versioning;

using FluentValidation;

using Marten;

using Npgsql;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using RecipeBook.Api.Infrastructure.Middleware;

namespace RecipeBook.Api.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRecipeBookDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RecipeBook")
                               ?? throw new InvalidOperationException("Connection string not found");

        services.AddMarten(opts =>
            {
                opts.Connection(connectionString);
                opts.DatabaseSchemaName = "recipebook";
                opts.ApplicationAssembly = typeof(IRecipeBookApiMarker).Assembly;
            })
            .UseLightweightSessions();

        return services;
    }

    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            });

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static IServiceCollection AddRecipeBookApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IServiceCollection AddRecipeBook(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IRecipeBookApiMarker>();

        return services;
    }

    public static IServiceCollection AddRecipeBookCaching(this IServiceCollection services, IConfiguration config)
    {
        var cache = config.GetConnectionString("Cache")
                    ?? throw new NullReferenceException("The Cache connection string is missing.");

        services.AddStackExchangeRedisCache(options => options.Configuration = cache);

        return services;
    }

    public static IServiceCollection AddObservability(this IServiceCollection services, string applicationName)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: applicationName,
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

        return services;
    }
}