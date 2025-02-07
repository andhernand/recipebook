using FluentValidation;

namespace RecipeBook.Api.Filters;

public class RequestValidationFilter<T>(
    IValidator<T> validator,
    ILogger<RequestValidationFilter<T>> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().First();
        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            var problems = TypedResults.ValidationProblem(result.ToDictionary());
            logger.LogInformation("The request had the following {@Problems}", problems);
            return problems;
        }

        return await next(context);
    }
}