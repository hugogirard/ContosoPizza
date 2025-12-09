
namespace Contoso.MCP.Menu.Services;

public interface IMenuRepository
{
    Task<IEnumerable<Item>> GetMenuAsync(ItemType? type);

    Task<IEnumerable<Item>> GetItemsByIdAsync(IEnumerable<string> ids);
}