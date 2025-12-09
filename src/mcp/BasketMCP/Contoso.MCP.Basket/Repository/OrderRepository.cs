using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.MCP.Order.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly TableClient _table;

    public OrderRepository()
    {
        var serviceClient = new TableServiceClient(
            new Uri(Environment.GetEnvironmentVariable("StorageAccountUri")),
            new TableSharedKeyCredential(Environment.GetEnvironmentVariable("StorageAccountName"),
                                         Environment.GetEnvironmentVariable("StorageAccountKey")));

        _table = serviceClient.GetTableClient("basket");
    }

    public async Task<Models.Order> CreateNewOrderAsync(string userId)
    {
        string orderId = Guid.NewGuid().ToString();

        Models.Order order = new Models.Order
        {
            PartitionKey = userId,
            RowKey = orderId
        };

        var response = await _table.UpsertEntityAsync<Models.Order>(entity: order, mode: TableUpdateMode.Replace);

        if (response.IsError)
        {
            throw new Exception(response.ReasonPhrase);
        }

        return order;
    }

    public async Task<Models.Order> AddItemAsync(string userId, string orderId, string itemId)
    {
        var order = await GetCurrentItems(userId, orderId);

        order.Items.Add(itemId);

        return await UpsertOrder(order);
    }

    public async Task<Models.Order> AddItemsAsync(string userId, string orderId, IEnumerable<string> itemIds)
    {
        var order = await GetCurrentItems(userId, orderId);

        order.Items.AddRange(itemIds);

        return await UpsertOrder(order);
    }

    public async Task<Models.Order> RemoveItemAsync(string userId, string orderId, string itemId)
    {
        var order = await GetCurrentItems(userId, orderId);

        order.Items.Remove(itemId);

        return await UpsertOrder(order);
    }

    public async Task<IEnumerable<Models.Order>> GetAllOrdersAsync(string userId)
    {
        AsyncPageable<Models.Order> results = _table.QueryAsync<Models.Order>(
            order => order.PartitionKey == userId);

        List<Models.Order> orders = new();

        await foreach (var order in results)
        {
            orders.Add(order);
        }

        return orders;
    }

    public async Task<Models.Order> GetOrderByIdAsync(string userId, string orderId)
    {
        var response = await _table.GetEntityAsync<Models.Order>(userId, orderId);

        return response.Value;
    }

    public async Task DeleteOrderAsync(string userId, string orderId) 
    {
        var response = await _table.GetEntityAsync<Models.Order>(userId, orderId);

        await _table.DeleteEntityAsync(response.Value);
    }

    private async Task<Models.Order> UpsertOrder(Models.Order order)
    {
        var response = await _table.UpsertEntityAsync<Models.Order>(entity: order, mode: TableUpdateMode.Replace);

        if (response.IsError)
        {
            throw new Exception(response.ReasonPhrase);
        }

        return order;
    }

    private async Task<Models.Order> GetCurrentItems(string userId, string orderId)
    {
        var response = await _table.GetEntityAsync<Models.Order>(userId, orderId);

        if (response.Value == null)
            return await CreateNewOrderAsync(userId);

        return response.Value;
    }
}
