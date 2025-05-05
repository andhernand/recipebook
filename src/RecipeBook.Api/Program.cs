using RecipeBook.Api.Infrastructure.Extensions;

using Scalar.AspNetCore;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddApiServices()
    .AddErrorHandling()
    .AddObservability()
    .AddDatabase()
    .AddApiVersioning()
    .AddApplicationServices()
    .AddCaching();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.MapEndpoints();

await app.RunAsync();