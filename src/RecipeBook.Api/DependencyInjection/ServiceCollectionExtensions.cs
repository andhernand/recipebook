using Marten;

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
}