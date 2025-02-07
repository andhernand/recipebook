using Marten;

namespace RecipeBook.Api.Services;

public class Service<TEntity> : IService<TEntity> where TEntity : class
{
    private readonly IDocumentSession _session;

    public Service(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.LoadAsync<TEntity>(id, cancellationToken);
    }
}