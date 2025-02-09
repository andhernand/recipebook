using System.Net;
using System.Net.Http.Json;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class GetRecipeByIdTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task GetRecipeById_WhenRecipeDoesNotExist_ReturnsNotFound()
    {
        using var client = CreateClient();

        var id = Ulid.NewUlid().ToGuid();

        var response = await client.GetAsync($"{Mother.RecipesApiPath}/{id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeExists_ReturnsRecipe()
    {
        using var client = CreateClient();

        var expected = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(expected.Id);

        var response = await client.GetAsync(
            $"{Mother.RecipesApiPath}/{expected.Id}",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var actual = await response.Content.ReadFromJsonAsync<Recipe>(TestContext.Current.CancellationToken);
        actual.ShouldBeEquivalentTo(expected);
    }
}