using System.Net;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class DeleteRecipeByIdTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task DeleteRecipeById_WhenRecipeDoesNotExist_ShouldReturnNotFound()
    {
        using var client = CreateClient();

        var id = Guid.CreateVersion7();

        var response = await client.DeleteAsync($"{Mother.RecipesApiPath}/{id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipeById_WhenRecipeDoesExist_ShouldReturnNoContent()
    {
        using var client = CreateClient();

        var recipe = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(recipe.Id);

        var response = await client.DeleteAsync(
            $"{Mother.RecipesApiPath}/{recipe.Id}",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}