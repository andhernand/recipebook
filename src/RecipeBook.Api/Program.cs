using RecipeBook.Api.Infrastructure.DependencyInjection;
using RecipeBook.Api.Infrastructure.OpenApi;
using RecipeBook.Api.Infrastructure.RequestPipeline;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logConfig) =>
    logConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi("v1", opts =>
{
    opts.AddDocumentTransformer<RecipeBookDocumentTransformer>();
    opts.ShouldInclude = description => description.GroupName is "v1";
});

builder.Services.AddGlobalErrorHandling();
builder.Services.AddRecipeBookDatabase(builder.Configuration);
builder.Services.AddRecipeBookApiVersioning();
builder.Services.AddRecipeBook();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseGlobalErrorHandling();
app.UseSerilogRequestLogging();

app.MapRecipeBookEndpoints();

app.Run();