using Contoso.MCP.Order.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.MCP.Order.Models;

namespace Contoso.MCP.Order;

public class OrderTool
{
    private readonly ILogger<OrderTool> _logger;
    private readonly IOrderRepository _orderRepository;

    private const string DEV_USER_ID = "testuser";

    public OrderTool(IOrderRepository orderRepository, ILogger<OrderTool> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    [Function("newOrder")]
    public async Task<Models.Order> NewOrder([McpToolTrigger("newOrder", "Create a new order for customer")] ToolInvocationContext context)
    {
        try
        {
            return await _orderRepository.CreateNewOrderAsync(DEV_USER_ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving orders.");
        }
    }

    [Function("deleteOrder")]
    public async Task<string> DeleteOrder([McpToolTrigger("deleteOrder", "Delete an order of customer")] ToolInvocationContext context,
                                         [McpToolProperty("orderId", "The orderId", isRequired: true)] string orderId)
    {
        try
        {
            await _orderRepository.DeleteOrderAsync(DEV_USER_ID, orderId);
            return $"Order {orderId} deleted";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving orders.");
        }
    }

    [Function("getOrder")]
    public async Task<IEnumerable<Models.Order>> GetOrder([McpToolTrigger("getOrder", "Get all the orders of a customer")] ToolInvocationContext context)
    {
        try
        {
            return await _orderRepository.GetAllOrdersAsync(DEV_USER_ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving orders.");
        }
    }

    [Function("getOrderById")]
    public async Task<Models.Order> GetOrderById([McpToolTrigger("getOrderById", "Get the order of a customer by orderId")] ToolInvocationContext context,
                                          [McpToolProperty("orderId", "The orderId", isRequired: true)] string orderId)
    {
        try
        {
            return await _orderRepository.GetOrderByIdAsync(DEV_USER_ID, orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving menu.");
        }
    }

    [Function("addItemOrder")]
    public async Task<Models.Order> AddItemOrder([McpToolTrigger("addItemOrder", "Add an item to a specific order")] ToolInvocationContext context,
                                                         [McpToolProperty("orderId", "The orderId", isRequired: true)] string orderId,
                                                         [McpToolProperty("itemId", "The itemId of the item to add to the order", isRequired: true)] string itemId)
    {
        try
        {
            return await _orderRepository.AddItemAsync(DEV_USER_ID, orderId, itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving menu.");
        }
    }

    [Function("addItemsOrder")]
    public async Task<Models.Order> AddItemsOrder([McpToolTrigger("addItemsOrder", "Add items to a specific order")] ToolInvocationContext context,
                                                         [McpToolProperty("orderId", "The orderId", isRequired: true)] string orderId,
                                                         [McpToolProperty("itemsId", "The item ids to add to the order", isRequired: true)] IEnumerable<string> itemIds)
    {
        try
        {
            return await _orderRepository.AddItemsAsync(DEV_USER_ID, orderId, itemIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving menu.");
        }
    }

    [Function("removeItemOrder")]
    public async Task<Models.Order> RemoveItemOrder([McpToolTrigger("removeItemOrder", "Remove item from a specific order")] ToolInvocationContext context,
                                                         [McpToolProperty("orderId", "The orderId", isRequired: true)] string orderId,
                                                         [McpToolProperty("itemId", "The item id to remove from the order", isRequired: true)] string itemId)
    {
        try
        {
            return await _orderRepository.RemoveItemAsync(DEV_USER_ID, orderId, itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception("An error occurred while retrieving menu.");
        }
    }
}



