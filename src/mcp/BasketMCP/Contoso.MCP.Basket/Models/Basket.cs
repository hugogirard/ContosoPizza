namespace Contoso.MCP.Menu.Models;

public record Basket(string Id, string UserId, IEnumerable<string> itemIds);
