using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

namespace RecipeBook.Api.IntegrationTests;

public class RecipeBookApiFactory : WebApplicationFactory<IRecipeBookApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17.2")
        .WithPassword("Sup3r!S3cr3t!T35ting")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:RecipeBook", _postgres.GetConnectionString());
        base.ConfigureWebHost(builder);
    }

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
    }
}