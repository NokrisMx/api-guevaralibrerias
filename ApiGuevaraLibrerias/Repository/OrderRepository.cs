using ApiGuevaraLibrerias.Models;
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

    public async Task<IEnumerable<Order>> GetOrders()
    {
        return await _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrder(int id)
    {
        return await _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrder(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
        return order;
    }
}