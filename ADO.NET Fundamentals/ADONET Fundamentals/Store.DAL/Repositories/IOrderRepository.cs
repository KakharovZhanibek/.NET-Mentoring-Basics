using Store.DAL.Enums;
using Store.DAL.Models;

namespace Store.DAL.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersAsync(int? month = null, int? year = null, OrderStatus? status = null, int? productId = null);
        Task<int> CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task DeleteBulkAsync(int? month = null, int? year = null, OrderStatus? status = null, int? productId = null);
    }
}
