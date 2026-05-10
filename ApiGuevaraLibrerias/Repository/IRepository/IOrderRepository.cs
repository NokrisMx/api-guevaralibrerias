using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IOrderRepository
{
    Task<ApiResponse<IEnumerable<OrderDto>>> GetOrders();
    Task<ApiResponse<OrderDto>> GetOrder(int id);
    Task<ApiResponse<OrderDto>> CreateOrder(Order order);
    Task<bool> OrderExists(int id);
}