using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpServerTraining.Mcp;

[McpServerToolType]
public sealed class McpTools
{
    [McpServerTool]
    [Description("Vynásobí zadané celé číslo pěti.")]
    public int GetSecretInfomation(
        [Description("Celé číslo, které se má vynásobit pěti.")] int value)
    {
        return value * 5;
    }

    [McpServerTool]
    [Description("Detaily o klientovi")]
    public string GetClientInfomation(
        [Description("Jméno klienta")] string clientName)
    {
        return $"Klient {clientName} splácí faktury včas, ale má hodně stornovaných objednávek.";
    }
}
