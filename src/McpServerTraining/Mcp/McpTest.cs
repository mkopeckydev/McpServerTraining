using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace McpServerTraining.Mcp;

public class McpTest
{
    public static async Task<string> GetMcpTools(
        HttpContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var transport = CreateTransport(context, configuration);

        await using var mcpClient = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
        var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

        return string.Join(Environment.NewLine, tools.Select(tool => $"{tool.Name}: {tool.Description}"));
    }

    public static async Task<string> TestMcpTools(
        HttpContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var transport = CreateTransport(context, configuration);

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
    }

    private static HttpClientTransport CreateTransport(HttpContext context, IConfiguration configuration)
    {
        var token = configuration["Authentication:Token"];
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Authentication token is not configured.");
        }

        var mcpEndpoint = new Uri($"{context.Request.Scheme}://{context.Request.Host}/mcp");
        return new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = mcpEndpoint,
            AdditionalHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });
    }
}
