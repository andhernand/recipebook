using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.Api.Models;
using RecipeBook.Api.Services;

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
                    IService<Recipe> service,
                    CancellationToken token = default) =>
                {
                    var recipe = await service.GetByIdAsync(id, token);

                    return recipe is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(recipe);
                })
            .WithName(Name)
            .WithDescription(Description);

        return group;
    }
}