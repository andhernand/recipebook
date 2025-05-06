using Oakton;

using RecipeBook.Api.Infrastructure.Extensions;

using Scalar.AspNetCore;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.ApplyOaktonExtensions();

builder
    .AddLogging()
    .AddApiServices()
    .AddErrorHandling()
    .AddObservability()
    .AddDatabase()
    .AddApiVersioning()
    .AddApplicationServices();

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

return await app.RunOaktonCommands(args);