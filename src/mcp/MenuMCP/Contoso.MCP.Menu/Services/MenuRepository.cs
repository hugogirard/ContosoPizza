using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.MCP.Menu.Services;

public class MenuRepository : IMenuRepository
{
    private readonly Container _container;

    public MenuRepository()
    {
        var client = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDBConnectionString"));
        var db = client.GetDatabase("contosopizza");
        _container = db.GetContainer("menu");
    }

    public async Task<IEnumerable<Item>> GetMenu(ItemType? type)
    {
        var items = new List<Item>();
        QueryDefinition query;

        if (type is null)
        {
            query = new QueryDefinition("SELECT * FROM c");
        }
        else
        {
            query = new QueryDefinition("SELECT * FROM c where c.type = @type").WithParameter("@type", type);
        }

        var feeds = _container.GetItemQueryIterator<Item>(query);

        while (feeds.HasMoreResults)
        {
            var response = await feeds.ReadNextAsync();
            items.AddRange(response.Resource);
        }

        return items;
    }
}
