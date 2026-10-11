using McpServerTraining;
using McpServerTraining.Mcp;

var projectVersion = System.Reflection.Assembly.GetExecutingAssembly()
    .GetName()
    .Version?
    .ToString(3) ?? "1.0.0";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<McpTools>();

// Přidání služeb do kontejneru.
// Další informace o konfiguraci OpenAPI: https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "API serveru MCP";
        document.Info.Version = projectVersion;

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Konfigurace HTTP pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", $"API serveru MCP {projectVersion}");
    });
}

//app.UseHttpsRedirection();

app.MapMcp("/mcp");

EndpointMapper.MapEndpoints(app);

app.Run();
