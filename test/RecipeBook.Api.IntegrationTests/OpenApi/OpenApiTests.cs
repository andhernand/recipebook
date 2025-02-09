using System.Net;

namespace RecipeBook.Api.IntegrationTests.OpenApi;

public class OpenApiTests(RecipeBookApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task OpenApi_WhenNavigatedTo_ShouldReturnOk()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Scalar_WhenNavigatedTo_ShouldReturnOk()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/scalar", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}