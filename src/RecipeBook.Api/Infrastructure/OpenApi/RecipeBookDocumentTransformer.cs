using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace RecipeBook.Api.Infrastructure.OpenApi;

public class RecipeBookDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Recipe Book - Web API", Version = "v1", Description = "A Web API for storing Recipes."
        };

        return Task.CompletedTask;
    }
}