using FluentValidation;
using FluentValidation.Results;

namespace RecipeBook.Api.Infrastructure.Filters;

public class RequestValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<RequestValidationFilter<T>> _logger;

    public RequestValidationFilter(IValidator<T> validator, ILogger<RequestValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        T request = context.Arguments.OfType<T>().First();

        ValidationResult validationResult = await _validator.ValidateAsync(
            request,
            context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{Type} had the following {ValidationFailures}",
                typeof(T).Name,
                validationResult.ToString(","));

            return TypedResults.ValidationProblem(
                errors: validationResult.ToDictionary(),
                instance: $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}"
            );
        }

        return await next(context);
    }
}