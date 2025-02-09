using System.Net.Http.Json;

using Bogus;

using RecipeBook.Api.Recipes;

namespace RecipeBook.Api.IntegrationTests;

public static class Mother
{
    public const string RecipesApiPath = "/api/v1/recipes";

    public static CreateRecipeRequest GenerateCreateRecipeRequest(
        string? title = null,
        string? description = null,
        string? author = null,
        IEnumerable<string>? ingredients = null,
        IEnumerable<string>? instructions = null)
    {
        var recipeFaker = new Faker<CreateRecipeRequest>()
            .CustomInstantiator(f => new CreateRecipeRequest(
                title ?? f.Lorem.Sentence(3),
                description ?? f.Lorem.Sentence(3),
                author ?? f.Person.FullName,
                ingredients ?? f.Make(6, () => f.Lorem.Sentence(4)),
                instructions ?? f.Make(10, () => f.Lorem.Sentence(6))));

        return recipeFaker.Generate();
    }

    public static UpdateRecipeRequest GenerateUpdateRecipeRequest(
        string? title = null,
        string? description = null,
        string? author = null,
        IEnumerable<string>? ingredients = null,
        IEnumerable<string>? instructions = null)
    {
        var recipeFaker = new Faker<UpdateRecipeRequest>()
            .CustomInstantiator(f => new UpdateRecipeRequest(
                title ?? f.Lorem.Sentence(3),
                description ?? f.Lorem.Sentence(3),
                author ?? f.Person.FullName,
                ingredients ?? f.Make(6, () => f.Lorem.Sentence(4)),
                instructions ?? f.Make(10, () => f.Lorem.Sentence(6))));

        return recipeFaker.Generate();
    }

    public static async Task<Recipe> CreateRecipeAsync(HttpClient client, CancellationToken token = default)
    {
        var request = GenerateCreateRecipeRequest();
        var response = await client.PostAsJsonAsync(RecipesApiPath, request, token);
        var recipe = await response.Content.ReadFromJsonAsync<Recipe>(token);
        return recipe!;
    }
}