using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> OrderExists(int id)
    {
        return await _db.Orders.AnyAsync(o => o.Id == id);
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _db.Orders.Include(o => o.User).Include(o => o.OrderDetails).ThenInclude(od => od.Book).OrderByDescending(o => o.CreatedAt).ToListAsync();

        var orderDtos = orders.Select(MapOrderToDto);

        return new ApiResponse<IEnumerable<OrderDto>>
        {
            Success = true,
            Message = "Órdenes obtenidas correctamente.",
            Data = orderDtos
        };
    }

    public async Task<ApiResponse<OrderDto>> GetOrder(int id)
    {
        var order = await _db.Orders.Include(o => o.User).Include(o => o.OrderDetails).ThenInclude(od => od.Book).FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return new ApiResponse<OrderDto>
            {
                Success = false,
                Message = "Orden no encontrada.",
                Data = null
            };
        }

        return new ApiResponse<OrderDto>
        {
            Success = true,
            Message = "Orden obtenida correctamente.",
            Data = MapOrderToDto(order)
        };
    }

    public async Task<ApiResponse<OrderDto>> CreateOrder(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();

        var createdOrder = await _db.Orders.Include(o => o.User).Include(o => o.OrderDetails).ThenInclude(od => od.Book).FirstOrDefaultAsync(o => o.Id == order.Id);

        return new ApiResponse<OrderDto>
        {
            Success = true,
            Message = "Orden creada correctamente.",
            Data = MapOrderToDto(createdOrder!)
        };
    }

    private OrderDto MapOrderToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User?.UserName ?? string.Empty,
            CreatedAt = order.CreatedAt,
            Total = order.Total,
            Status = order.Status.ToString(),
            Items = order.OrderDetails.Select(od => new OrderDetailDto
            {
                BookId = od.BookId,
                Title = od.Book?.Title ?? string.Empty,
                Quantity = od.Quantity,
                Price = od.Price,
                SubTotal = od.Price * od.Quantity
            }).ToList()
        };
    }
}