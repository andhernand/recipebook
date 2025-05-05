using RecipeBook.Api.Infrastructure.DependencyInjection;
using RecipeBook.Api.Infrastructure.OpenApi;
using RecipeBook.Api.Infrastructure.RequestPipeline;

using Scalar.AspNetCore;

using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logConfig) =>
{
    logConfig.ReadFrom.Configuration(context.Configuration);
    logConfig.WriteTo.OpenTelemetry(
        endpoint: "http://localhost:4317",
        protocol: OtlpProtocol.Grpc,
        includedData: IncludedData.MessageTemplateTextAttribute
                      | IncludedData.MessageTemplateMD5HashAttribute
                      | IncludedData.TraceIdField
                      | IncludedData.SpanIdField
                      | IncludedData.SpecRequiredResourceAttributes
                      | IncludedData.SourceContextAttribute,
        resourceAttributes: new Dictionary<string, object>
        {
            { "service.name", builder.Environment.ApplicationName },
            { "service.instance.id", Environment.MachineName }
        });
});

builder.Services.AddOpenApi("v1", opts =>
{
    opts.AddDocumentTransformer<RecipeBookDocumentTransformer>();
    opts.ShouldInclude = description => description.GroupName is "v1";
});

builder.Services.AddGlobalErrorHandling();
builder.Services.AddObservability(builder.Environment.ApplicationName);
builder.Services.AddRecipeBookDatabase(builder.Configuration);
builder.Services.AddRecipeBookApiVersioning();
builder.Services.AddRecipeBook();
builder.Services.AddRecipeBookCaching(builder.Configuration);

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseGlobalErrorHandling();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.MapRecipeBookEndpoints();

await app.RunAsync();