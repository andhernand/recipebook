using RecipeBook.Api.DependencyInjection;
using RecipeBook.Api.OpenApi;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logConfig) =>
    logConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi("v1", opts =>
{
    opts.AddDocumentTransformer<RecipeBookDocumentTransformer>();
    opts.ShouldInclude = description => description.GroupName is "v1";
});

builder.Services.AddRecipeBookDatabase(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.Run();