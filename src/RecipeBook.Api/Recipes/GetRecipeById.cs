using Marten;

using Microsoft.AspNetCore.Http.HttpResults;

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
                    CancellationToken token = default) =>
                {
                    var recipe = await session.LoadAsync<Recipe>(id, token);

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