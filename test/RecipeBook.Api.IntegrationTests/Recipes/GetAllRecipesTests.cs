using System.Net;
using System.Net.Http.Json;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class GetAllRecipesTests : IClassFixture<RecipeBookApiFactory>
{
    private readonly RecipeBookApiFactory _factory;

    public GetAllRecipesTests(RecipeBookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllRecipes_WhenNoRecipesExist_ShouldReturnEmptyArray()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<Recipe[]>(TestContext.Current.CancellationToken);
        content.ShouldBeEmpty();
    }
}