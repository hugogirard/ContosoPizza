using System.Web;

namespace Contoso.MCP.Menu.Models;

public enum ItemType 
{ 
    MainDish = 0,
    Side,
    Drink
}

public record Item(string Id, string Name,ItemType type, IEnumerable<Ingredient> Ingredients, decimal Price);
