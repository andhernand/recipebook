using System.Diagnostics;

using Asp.Versioning;

using FluentValidation;

using Marten;

using Microsoft.AspNetCore.Http.Features;

using RecipeBook.Api.RequestPipeline;
using RecipeBook.Api.Services;

namespace RecipeBook.Api.DependencyInjection;

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
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

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
        services.AddScoped(typeof(IService<>), typeof(Service<>));

        services.AddValidatorsFromAssemblyContaining<IRecipeBookApiMarker>();

        return services;
    }
}