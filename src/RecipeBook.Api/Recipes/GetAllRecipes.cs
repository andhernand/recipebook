using Marten;

using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.Api.Models;

namespace RecipeBook.Api.Recipes;

public static class GetAllRecipes
{
    private const string Name = "GetAllRecipes";
    private const string Description = "Retrieve all recipes";
    private const string Route = "";

    public static RouteGroupBuilder MapGetAllRecipes(this RouteGroupBuilder group)
    {
        group.MapGet(Route,
                async Task<Ok<IReadOnlyList<Recipe>>> (
                    IQuerySession session,
                    CancellationToken token = default) =>
                {
                    var recipes = await session.Query<Recipe>().ToListAsync(token);

                    return TypedResults.Ok(recipes);
                })
            .WithName(Name)
            .WithDescription(Description)
            .MapToApiVersion(1);

        return group;
    }
}