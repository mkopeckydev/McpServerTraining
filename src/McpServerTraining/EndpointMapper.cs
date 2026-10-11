using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

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

        app.MapGet("/mcplist", async (HttpContext context, CancellationToken cancellationToken) =>
        {
            var mcpEndpoint = new Uri($"{context.Request.Scheme}://{context.Request.Host}/mcp");
            var transport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = mcpEndpoint
            });

            await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
            var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

            return string.Join(Environment.NewLine, tools.Select(tool => $"{tool.Name}: {tool.Description}"));
        })
        .WithName("GetMcpTools")
        .WithSummary("Výpis nástrojů MCP.")
        .WithDescription("Vrací seznam nástrojů dostupných na MCP endpointu /mcp.")
        .WithTags("MCP");

        app.MapGet("/mcptest", async (HttpContext context, CancellationToken cancellationToken) =>
        {
            var mcpEndpoint = new Uri($"{context.Request.Scheme}://{context.Request.Host}/mcp");
            var transport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = mcpEndpoint
            });

            await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);

            var secretResult = await mcpClient.CallToolAsync(
                "get_secret_infomation",
                new Dictionary<string, object?>
                {
                    ["value"] = 33
                },
                cancellationToken: cancellationToken);

            var secretText = "get_secret_infomation: " + string.Join(Environment.NewLine, secretResult.Content
                .OfType<TextContentBlock>()
                .Select(content => content.Text));

            var clientResult = await mcpClient.CallToolAsync(
                "get_client_infomation",
                new Dictionary<string, object?>
                {
                    ["clientName"] = "Valašská firma"
                },
                cancellationToken: cancellationToken);

            var clientText = "get_secret_infomation: " + string.Join(Environment.NewLine, clientResult.Content
                .OfType<TextContentBlock>()
                .Select(content => content.Text));

            return string.Join(Environment.NewLine, secretText, clientText);
        })
        .WithName("TestMcpTools")
        .WithSummary("Test nástrojů MCP.")
        .WithDescription("Postupně volá všechny nástroje MCP s testovacími vstupními hodnotami.")
        .WithTags("MCP");
    }
}
