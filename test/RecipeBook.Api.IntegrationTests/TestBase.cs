using Microsoft.AspNetCore.Mvc.Testing;

namespace RecipeBook.Api.IntegrationTests;

public class TestBase(RecipeBookApiFactory factory)
    : IClassFixture<RecipeBookApiFactory>, IAsyncLifetime
{
    protected readonly List<Guid> _recipeIds = [];

    protected HttpClient CreateClient()
    {
        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (_recipeIds.Count == 0)
            return;

        using var client = CreateClient();

        foreach (var recipeId in _recipeIds)
        {
            _ = await client.DeleteAsync($"{Mother.RecipesApiPath}/{recipeId}", TestContext.Current.CancellationToken);
        }
    }
}