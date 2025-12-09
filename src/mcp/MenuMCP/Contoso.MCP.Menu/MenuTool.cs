using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.MCP.Menu;

public class MenuTool
{
    private readonly ILogger<MenuTool> _logger;
    private readonly IMenuRepository _menuRepository;

    public MenuTool(IMenuRepository menuRepository, ILogger<MenuTool>  logger)
    {
        _menuRepository = menuRepository;
        _logger = logger;
    }

    [Function("getMenu")]
    public async Task<IEnumerable<Item>> GetMenu([McpToolTrigger("getMenu", "Get the complete menu for all item type or specific one")] ToolInvocationContext context,
                                                 [McpToolProperty("itemType", "The item type: 0=MainDish, 1=Side, 2=Drink", isRequired: false)] ItemType? itemType) 
    {
        try
        {
            return await _menuRepository.GetMenuAsync(itemType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving menu.");
        }
    }


    [Function("getItemById")]
    public async Task<IEnumerable<Item>> GetItemById([McpToolTrigger("getItemById", "Get specific item by list of ids")] ToolInvocationContext context,
                                                     [McpToolProperty("ids", "The list of item ids to get", isRequired: true)] IEnumerable<string> ids)
    {
        try
        {
            return await _menuRepository.GetItemsByIdAsync(ids);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving items.");
        }
    }
}
