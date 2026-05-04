using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders();
    Task<Order?> GetOrder(int id);
    Task<Order> CreateOrder(Order order);
    Task<bool> OrderExists(int id);
}