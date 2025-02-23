using Marten;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;

using RecipeBook.Api.Infrastructure.Caching;

namespace RecipeBook.Api.Recipes;

public static class GetRecipeById
{
    public const string Name = "GetRecipeById";
    private const string Description = "Retrieve a Recipe by Id";
    private const string Route = "/{id:guid}";

    public static RouteGroupBuilder MapGetRecipeById(this RouteGroupBuilder group)
    {
        group.MapGet(Route,
                async Task<Results<Ok<Recipe>, NotFound>> (
                    Guid id,
                    IQuerySession session,
                    IDistributedCache cache,
                    CancellationToken token = default) =>
                {
                    var recipe = await cache.GetAsync($"{nameof(Recipe)}-{id}",
                        async ct =>
                        {
                            var recipe = await session.LoadAsync<Recipe>(id, ct);
                            return recipe;
                        },
                        CacheOptions.DefaultExpiration,
                        token);

                    return recipe is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(recipe);
                })
            .WithName(Name)
            .WithDescription(Description)
            .MapToApiVersion(1);

        return group;
    }
}