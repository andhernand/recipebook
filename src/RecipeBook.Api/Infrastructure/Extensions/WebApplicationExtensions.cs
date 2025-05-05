using Asp.Versioning;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        app.MapGroup("api/v{version:apiVersion}/recipes")
            .MapGetRecipeById()
            .MapGetAllRecipes()
            .MapCreateRecipe()
            .MapDeleteRecipeById()
            .MapUpdateRecipe()
            .WithTags("Recipes")
            .WithApiVersionSet(apiVersionSet)
            .WithOpenApi();

        return app;
    }
}