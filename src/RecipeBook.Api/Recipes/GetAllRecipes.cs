using Marten;
using Marten.Pagination;

using Microsoft.AspNetCore.Http.HttpResults;

namespace RecipeBook.Api.Recipes;

public static class GetAllRecipes
{
    private const string Name = "GetAllRecipes";
    private const string Description = "Retrieve all recipes";
    private const string Route = "";

    public static RouteGroupBuilder MapGetAllRecipes(this RouteGroupBuilder group)
    {
        group.MapGet(Route,
                async Task<Ok<RecipesResponse>> (
                    IQuerySession session,
                    int pageNumber = 1,
                    int pageSize = 10,
                    CancellationToken token = default) =>
                {
                    var recipes = await session.Query<Recipe>()
                        .ToPagedListAsync(pageNumber, pageSize, token);

                    var response = new RecipesResponse(
                        recipes.AsEnumerable(),
                        recipes.PageNumber,
                        recipes.PageSize,
                        recipes.PageCount,
                        recipes.TotalItemCount,
                        recipes.HasPreviousPage,
                        recipes.HasNextPage,
                        recipes.IsFirstPage,
                        recipes.IsLastPage,
                        recipes.FirstItemOnPage,
                        recipes.LastItemOnPage);

                    return TypedResults.Ok(response);
                })
            .WithName(Name)
            .WithDescription(Description)
            .MapToApiVersion(1);

        return group;
    }
}

public record RecipesResponse(
    IEnumerable<Recipe> Items,
    long PageNumber,
    long PageSize,
    long PageCount,
    long TotalItems,
    bool HasPreviousPage,
    bool HasNextPage,
    bool IsFirstPage,
    bool IsLastPage,
    long FirstItemOnPage,
    long LastItemOnPage);