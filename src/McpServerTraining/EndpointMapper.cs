using McpServerTraining.Mcp;

namespace McpServerTraining;

public static class EndpointMapper
{
    public static void MapEndpoints(WebApplication app)
    {
        app.MapGet("/", () =>
        {
            return "MCP Server";
        })
        .WithName("GetRoot")
        .WithSummary("Název služby.")
        .WithDescription("Vrací název serveru MCP.")
        .WithTags("Služba");

        app.MapGet("/status", () =>
        {
            return $"Service is running: {DateTime.Now}";
        })
        .WithName("GetStatus")
        .WithSummary("Stav služby.")
        .WithDescription("Vrací aktuální stav serveru MCP a místní čas serveru.")
        .WithTags("Služba");

        app.MapGet("/mcplist", McpTest.GetMcpTools)
        .WithName("GetMcpTools")
        .WithSummary("Výpis nástrojů MCP.")
        .WithDescription("Vrací seznam nástrojů dostupných na MCP endpointu /mcp.")
        .WithTags("MCP");

        app.MapGet("/mcptest", McpTest.TestMcpTools)
        .WithName("TestMcpTools")
        .WithSummary("Test nástrojů MCP.")
        .WithDescription("Postupně volá všechny nástroje MCP s testovacími vstupními hodnotami.")
        .WithTags("MCP");
    }
}
