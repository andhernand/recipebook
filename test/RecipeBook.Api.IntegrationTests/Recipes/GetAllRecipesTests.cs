using System.Net;
using System.Net.Http.Json;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class GetAllRecipesTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task GetAllRecipes_WhenNoRecipesExist_ShouldReturnEmptyArray()
    {
        using var client = CreateClient();

        var response = await client.GetAsync(Mother.RecipesApiPath, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<Recipe[]>(TestContext.Current.CancellationToken);
        content.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllRecipes_WhenRecipesExist_ShouldReturnRecipes()
    {
        using var client = CreateClient();

        var recipe1 = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(recipe1.Id);
        var recipe2 = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(recipe2.Id);
        var recipe3 = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(recipe3.Id);

        var response = await client.GetAsync(Mother.RecipesApiPath, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var recipes = await response.Content.ReadFromJsonAsync<Recipe[]>(TestContext.Current.CancellationToken);
        recipes.ShouldNotBeEmpty();
        recipes.Length.ShouldBe(3);
    }
}