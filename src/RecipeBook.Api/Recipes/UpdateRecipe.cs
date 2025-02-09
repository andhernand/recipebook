using System.Net.Mime;

using FluentValidation;

using Marten;

using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.Api.Infrastructure.Filters;

namespace RecipeBook.Api.Recipes;

public static class UpdateRecipe
{
    private const string Name = "UpdateRecipe";
    private const string Description = "Updates a recipe";
    private const string Route = "/{id:guid}";

    public static RouteGroupBuilder MapUpdateRecipe(this RouteGroupBuilder group)
    {
        group.MapPut(Route,
                async Task<Results<Ok<Recipe>, NotFound, ValidationProblem>> (
                    Guid id,
                    UpdateRecipeRequest request,
                    IDocumentSession session,
                    CancellationToken token = default) =>
                {
                    var existingRecipe = await session.LoadAsync<Recipe>(id, token);

                    if (existingRecipe is null)
                    {
                        return TypedResults.NotFound();
                    }

                    var updated = existingRecipe with
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Author = request.Author,
                        Ingredients = request.Ingredients,
                        Instructions = request.Instructions
                    };

                    session.Update(updated);
                    await session.SaveChangesAsync(token);

                    return TypedResults.Ok(updated);
                })
            .WithName(Name)
            .WithDescription(Description)
            .Accepts<UpdateRecipeRequest>(false, MediaTypeNames.Application.Json)
            .AddEndpointFilter<RequestValidationFilter<UpdateRecipeRequest>>();

        return group;
    }
}

public record UpdateRecipeRequest(
    string Title,
    string Description,
    string Author,
    IEnumerable<string> Ingredients,
    IEnumerable<string> Instructions);

public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Title)
            .Length(4, 256);

        RuleFor(x => x.Description)
            .Length(4, 256);

        RuleFor(x => x.Author)
            .Length(4, 256);

        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Ingredients).NotEmpty();
            });

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleForEach(x => x.Instructions).NotEmpty();
            });
    }
}