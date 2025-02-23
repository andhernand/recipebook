using Marten;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;

namespace RecipeBook.Api.Recipes;

public static class DeleteRecipeById
{
    private const string Name = "DeleteRecipeById";
    private const string Description = "Delete a Recipe by Id";
    private const string Route = "/{id:guid}";

    public static RouteGroupBuilder MapDeleteRecipeById(this RouteGroupBuilder group)
    {
        group.MapDelete(Route,
                async Task<Results<NoContent, NotFound>> (
                    Guid id,
                    IDocumentSession session,
                    IDistributedCache cache,
                    CancellationToken token = default) =>
                {
                    var recipe = await session.LoadAsync<Recipe>(id, token);

                    if (recipe is null)
                        return TypedResults.NotFound();

                    session.Delete(recipe);
                    await session.SaveChangesAsync(token);

                    await cache.RemoveAsync($"{nameof(Recipe)}-{id}", token);

                    return TypedResults.NoContent();
                })
            .WithName(Name)
            .WithDescription(Description)
            .MapToApiVersion(1);

        return group;
    }
}