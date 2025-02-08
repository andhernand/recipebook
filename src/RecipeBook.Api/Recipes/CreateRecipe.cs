using System.Net.Mime;

using FluentValidation;

using Marten;
using Marten.Schema.Identity;

using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.Api.Infrastructure.Filters;

namespace RecipeBook.Api.Recipes;

public static class CreateRecipe
{
    private const string Name = "CreateRecipe";
    private const string Description = "Creates a new recipe";
    private const string Route = "/";

    public static RouteGroupBuilder MapCreateRecipe(this RouteGroupBuilder group)
    {
        group.MapPost(Route,
                async Task<Results<CreatedAtRoute<Recipe>, ValidationProblem>> (
                    CreateRecipeRequest request,
                    IDocumentSession session,
                    CancellationToken token = default) =>
                {
                    var id = CombGuidIdGeneration.NewGuid();
                    var recipe = new Recipe(
                        id,
                        request.Title,
                        request.Description,
                        request.Author,
                        request.Ingredients,
                        request.Instructions);

                    session.Store(recipe);
                    await session.SaveChangesAsync(token);

                    return TypedResults.CreatedAtRoute(recipe, GetRecipeById.Name, new { id = recipe.Id });
                })
            .WithName(Name)
            .WithDescription(Description)
            .Accepts<CreateRecipeRequest>(false, MediaTypeNames.Application.Json)
            .AddEndpointFilter<RequestValidationFilter<CreateRecipeRequest>>();

        return group;
    }
}

public record CreateRecipeRequest(
    string Title,
    string Description,
    string Author,
    IEnumerable<string> Ingredients,
    IEnumerable<string> Instructions);

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Author).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty();
        RuleForEach(x => x.Ingredients).NotEmpty();
        RuleFor(x => x.Instructions).NotEmpty();
        RuleForEach(x => x.Instructions).NotEmpty();
    }
}