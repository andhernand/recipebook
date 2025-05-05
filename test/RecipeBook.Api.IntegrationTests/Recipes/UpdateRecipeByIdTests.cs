using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests.Recipes;

public class UpdateRecipeByIdTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task UpdateRecipe_WhenRecipeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        using var client = CreateClient();

        var id = Guid.CreateVersion7();
        var request = Mother.GenerateUpdateRecipeRequest();

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRequestIsValid_ShouldCreateRecipe()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            "Andres Hernandez",
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var recipe = await response.Content.ReadFromJsonAsync<Recipe>(TestContext.Current.CancellationToken);
        recipe.ShouldNotBeNull();
        recipe.Id.ShouldBe(original.Id);
        recipe.Title.ShouldBe(request.Title);
        recipe.Description.ShouldBe(request.Description);
        recipe.Author.ShouldBe(request.Author);
        recipe.Ingredients.ShouldBe(request.Ingredients);
        recipe.Instructions.ShouldBe(request.Instructions);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeTitleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            string.Empty,
            original.Description,
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenRecipeTitleIsLessThanFourCharacters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            "abc",
            original.Description,
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenRecipeTitleIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            "a".PadRight(290, 'a'),
            original.Description,
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Title");
        problems.Errors["Title"].Length.ShouldBe(1);
        problems.Errors["Title"][0]
            .ShouldBe("'Title' must be between 4 and 256 characters. You entered 290 characters.");
    }

    [Fact]
    public async Task UpdateRecipe_WhenDescriptionIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            string.Empty,
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenDescriptionIsLessThanFourCharacters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            "abc",
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenDescriptionIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            "a".PadRight(275, 'a'),
            original.Author,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Description");
        problems.Errors["Description"].Length.ShouldBe(1);
        problems.Errors["Description"][0]
            .ShouldBe("'Description' must be between 4 and 256 characters. You entered 275 characters.");
    }

    [Fact]
    public async Task UpdateRecipe_WhenAuthorIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            string.Empty,
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenAuthorIsLessThan4Characters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            "ab",
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Author");
        problems.Errors["Author"].Length.ShouldBe(1);
        problems.Errors["Author"][0]
            .ShouldBe("'Author' must be between 4 and 256 characters. You entered 2 characters.");
    }

    [Fact]
    public async Task UpdateRecipe_WhenAuthorIsGreaterThan256Characters_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            "a".PadRight(265, 'a'),
            original.Ingredients,
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problems = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problems.ShouldNotBeNull();
        problems.Errors.Count.ShouldBe(1);
        problems.Errors.ShouldContainKey("Author");
        problems.Errors["Author"].Length.ShouldBe(1);
        problems.Errors["Author"][0]
            .ShouldBe("'Author' must be between 4 and 256 characters. You entered 265 characters.");
    }

    [Fact]
    public async Task UpdateRecipe_WhenIngredientsIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            original.Author,
            [],
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenIngredientsContainsAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            original.Author,
            [""],
            original.Instructions);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenInstructionsIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            original.Author,
            original.Ingredients,
            []);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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
    public async Task UpdateRecipe_WhenInstructionsContainsAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = CreateClient();

        var original = await Mother.CreateRecipeAsync(client, TestContext.Current.CancellationToken);
        _recipeIds.Add(original.Id);

        var request = Mother.GenerateUpdateRecipeRequest(
            original.Title,
            original.Description,
            original.Author,
            original.Ingredients,
            [""]);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{Mother.RecipesApiPath}/{original.Id}",
            request,
            TestContext.Current.CancellationToken);

        // Assert
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