using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class CreateRecipeTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task CreateRecipe_WhenRequestIsValid_ShouldCreateRecipe()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest();

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath,
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();

        var recipe = await response.Content.ReadFromJsonAsync<Recipe>(TestContext.Current.CancellationToken);
        recipe.ShouldNotBeNull();
        _recipeIds.Add(recipe.Id);
        recipe.Id.ShouldNotBe(Guid.Empty);
        recipe.Title.ShouldBe(request.Title);
        recipe.Description.ShouldBe(request.Description);
        recipe.Author.ShouldBe(request.Author);
        recipe.Ingredients.ShouldBeEquivalentTo(request.Ingredients);
        recipe.Instructions.ShouldBeEquivalentTo(request.Instructions);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeTitleIsEmpty_ShouldReturnBadRequest()
    {
        using var client = CreateClient();
        var request = Mother.GenerateCreateRecipeRequest(title: "");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Title");
        problems.Errors["Title"].Length.ShouldBe(1);
        problems.Errors["Title"][0].ShouldBe("'Title' must be between 4 and 256 characters. You entered 0 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeTitleIsLessThanFourCharacters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();
        var request = Mother.GenerateCreateRecipeRequest(title: "asb");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Title");
        problems.Errors["Title"].Length.ShouldBe(1);
        problems.Errors["Title"][0].ShouldBe("'Title' must be between 4 and 256 characters. You entered 3 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeTitleIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();
        var request = Mother.GenerateCreateRecipeRequest(title: "a".PadRight(300, 'a'));

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Title");
        problems.Errors["Title"].Length.ShouldBe(1);
        problems.Errors["Title"][0]
            .ShouldBe("'Title' must be between 4 and 256 characters. You entered 300 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenDescriptionIsEmpty_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(description: "");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Description");
        problems.Errors["Description"].Length.ShouldBe(1);
        problems.Errors["Description"][0]
            .ShouldBe("'Description' must be between 4 and 256 characters. You entered 0 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenDescriptionIsLessThanFourCharacters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(description: "abc");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Description");
        problems.Errors["Description"].Length.ShouldBe(1);
        problems.Errors["Description"][0]
            .ShouldBe("'Description' must be between 4 and 256 characters. You entered 3 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenDescriptionIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(description: "a".PadRight(300, 'a'));

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Description");
        problems.Errors["Description"].Length.ShouldBe(1);
        problems.Errors["Description"][0]
            .ShouldBe("'Description' must be between 4 and 256 characters. You entered 300 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenAuthorIsEmpty_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(author: "");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Author");
        problems.Errors["Author"].Length.ShouldBe(1);
        problems.Errors["Author"][0]
            .ShouldBe("'Author' must be between 4 and 256 characters. You entered 0 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenAuthorIsLessThan4Characters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(author: "abc");

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Author");
        problems.Errors["Author"].Length.ShouldBe(1);
        problems.Errors["Author"][0]
            .ShouldBe("'Author' must be between 4 and 256 characters. You entered 3 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenAuthorIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(author: "a".PadRight(257, 'a'));

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Author");
        problems.Errors["Author"].Length.ShouldBe(1);
        problems.Errors["Author"][0]
            .ShouldBe("'Author' must be between 4 and 256 characters. You entered 257 characters.");
    }

    [Fact]
    public async Task CreateRecipe_WhenIngredientsIsEmpty_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(ingredients: []);

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Ingredients");
        problems.Errors["Ingredients"].Length.ShouldBe(1);
        problems.Errors["Ingredients"][0]
            .ShouldBe("'Ingredients' must not be empty.");
    }

    [Fact]
    public async Task CreateRecipe_WhenIngredientsContainsAnEmptyString_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(ingredients: [""]);

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Ingredients[0]");
        problems.Errors["Ingredients[0]"].Length.ShouldBe(1);
        problems.Errors["Ingredients[0]"][0]
            .ShouldBe("'Ingredients' must not be empty.");
    }

    [Fact]
    public async Task CreateRecipe_WhenInstructionsIsEmpty_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(instructions: []);

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Instructions");
        problems.Errors["Instructions"].Length.ShouldBe(1);
        problems.Errors["Instructions"][0]
            .ShouldBe("'Instructions' must not be empty.");
    }

    [Fact]
    public async Task CreateRecipe_WhenInstructionsContainsAnEmptyString_ShouldReturnBadRequest()
    {
        using var client = CreateClient();

        var request = Mother.GenerateCreateRecipeRequest(instructions: [""]);

        var response = await client.PostAsJsonAsync(
            Mother.RecipesApiPath, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Instructions[0]");
        problems.Errors["Instructions[0]"].Length.ShouldBe(1);
        problems.Errors["Instructions[0]"][0]
            .ShouldBe("'Instructions' must not be empty.");
    }
}