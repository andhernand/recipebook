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
        var content = await response.Content.ReadFromJsonAsync<RecipesResponse>(TestContext.Current.CancellationToken);
        content.ShouldNotBeNull();
        content.Items.ShouldNotBeNull();
        content.Items.ShouldBeEmpty();
        content.PageNumber.ShouldBe(1);
        content.PageSize.ShouldBe(10);
        content.PageCount.ShouldBe(0);
        content.TotalItems.ShouldBe(0);
        content.HasPreviousPage.ShouldBeFalse();
        content.HasNextPage.ShouldBeFalse();
        content.IsFirstPage.ShouldBeFalse();
        content.IsLastPage.ShouldBeFalse();
        content.FirstItemOnPage.ShouldBe(0);
        content.LastItemOnPage.ShouldBe(0);
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
        var content = await response.Content.ReadFromJsonAsync<RecipesResponse>(TestContext.Current.CancellationToken);
        // recipes.ShouldNotBeEmpty();
        // recipes.Length.ShouldBe(3);
        content.ShouldNotBeNull();
        content.Items.ShouldNotBeNull();
        content.Items.ShouldNotBeEmpty();
        content.Items.Count().ShouldBe(3);
        content.PageNumber.ShouldBe(1);
        content.PageSize.ShouldBe(10);
        content.PageCount.ShouldBe(1);
        content.TotalItems.ShouldBe(3);
        content.HasPreviousPage.ShouldBeFalse();
        content.HasNextPage.ShouldBeFalse();
        content.IsFirstPage.ShouldBeTrue();
        content.IsLastPage.ShouldBeTrue();
        content.FirstItemOnPage.ShouldBe(1);
        content.LastItemOnPage.ShouldBe(3);
    }
}