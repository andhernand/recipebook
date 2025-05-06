using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

namespace RecipeBook.Api.IntegrationTests;

public class RecipeBookApiFactory : WebApplicationFactory<IRecipeBookApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17.4")
        .WithPassword("Sup3r!S3cr3t!T35ting")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:RecipeBook", _postgres.GetConnectionString());

        base.ConfigureWebHost(builder);
    }

    public async ValueTask InitializeAsync()
    {
        Oakton.OaktonEnvironment.AutoStartHost = true;

        await _postgres.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgres.StopAsync();

        await _postgres.DisposeAsync();

        await base.DisposeAsync();
    }
}