using System.Net;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class DeleteRecipeByIdTests : IClassFixture<RecipeBookApiFactory>, IAsyncLifetime
{
    private readonly RecipeBookApiFactory _factory;
    private readonly List<Guid> _recipeIds = [];

    public DeleteRecipeByIdTests(RecipeBookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteRecipeById_WhenRecipeDoesNotExist_ShouldReturnNotFound()
    {
        using var client = _factory.CreateClient();
        var id = Ulid.NewUlid().ToGuid();

        var response = await client.DeleteAsync($"{Mother.RecipesApiPath}/{id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipeById_WhenRecipeDoesExist_ShouldReturnNoContent()
    {
        using var client = _factory.CreateClient();
        var recipe = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(recipe.Id);

        var response = await client.DeleteAsync(
            $"{Mother.RecipesApiPath}/{recipe.Id}",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (_recipeIds.Count == 0)
            return;

        using var client = _factory.CreateClient();

        foreach (var recipeId in _recipeIds)
        {
            _ = await client.DeleteAsync($"{Mother.RecipesApiPath}/{recipeId}", TestContext.Current.CancellationToken);
        }
    }
}