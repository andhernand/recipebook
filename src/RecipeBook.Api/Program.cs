using Marten;

using RecipeBook.Api;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logConfig) =>
    logConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddOpenApi();

builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RecipeBook")
                               ?? throw new InvalidOperationException("Connection string not found");

        opts.Connection(connectionString);
        opts.DatabaseSchemaName = "recipebook";
        opts.ApplicationAssembly = typeof(IRecipeBookApiMarker).Assembly;
    })
    .UseLightweightSessions();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.Run();