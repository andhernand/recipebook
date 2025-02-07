namespace RecipeBook.Api.Models;

public record Recipe(
    Guid Id,
    string Title,
    string Description,
    string Author,
    IEnumerable<string> Ingredients,
    IEnumerable<string> Instructions);