using Asp.Versioning;

namespace RecipeBook.Api.RequestPipeline;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        return app;
    }

    public static WebApplication MapRecipeBookEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        app.MapGroup("api/v{version:apiVersion}/recipes")
            .WithTags("Recipes")
            .WithApiVersionSet(apiVersionSet)
            .WithOpenApi();

        return app;
    }
}