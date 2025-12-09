
namespace Contoso.MCP.Menu.Services;

public interface IMenuRepository
{
    Task<IEnumerable<Item>> GetMenu(ItemType? type);
}