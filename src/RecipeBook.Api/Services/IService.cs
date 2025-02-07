namespace RecipeBook.Api.Services;

public interface IService<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}