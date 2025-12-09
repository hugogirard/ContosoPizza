
namespace Contoso.MCP.Order.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Models.Order>> GetAllOrdersAsync(string userId);
        Task<Models.Order> GetOrderByIdAsync(string userId, string orderId);
        Task<Models.Order> AddItemAsync(string userId, string orderId, string itemId);
        Task<Models.Order> AddItemsAsync(string userId, string orderId, IEnumerable<string> itemIds);
        Task<Models.Order> CreateNewOrderAsync(string userId);
        Task<Models.Order> RemoveItemAsync(string userId, string orderId, string itemId);
        Task DeleteOrderAsync(string userId, string orderId);
    }
}