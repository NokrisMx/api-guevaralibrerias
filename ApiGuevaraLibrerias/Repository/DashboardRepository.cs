using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _db = context;
        _userManager = userManager;
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetStats()
    {
        var books = await _db.Books.CountAsync();
        var authors = await _db.Authors.CountAsync();
        var categories = await _db.Categories.CountAsync();
        var publishers = await _db.Publishers.CountAsync();
        var orders = await _db.Orders.CountAsync();
        var users = await _userManager.Users.CountAsync();
        var revenue = await _db.Orders.Where(o => o.Status == OrderStatus.Paid).SumAsync(o => o.Total);

        if (books == 0 && authors == 0 && categories == 0 && publishers == 0 && orders == 0 && users == 0 && revenue == 0)
        {
            return new ApiResponse<DashboardStatsDto>
            {
                Success = false,
                Message = "No se encontraron estadísticas.",
                Data = null
            };
        }

        return new ApiResponse<DashboardStatsDto>
        {
            Success = true,
            Message = "Estadísticas obtenidas correctamente.",
            Data = new DashboardStatsDto
            {
                Books = books,
                Authors = authors,
                Categories = categories,
                Publishers = publishers,
                Orders = orders,
                Users = users,
                Revenue = revenue
            }
        };
    }

    public async Task<ApiResponse<IEnumerable<RecentOrderDto>>> GetRecentOrders(int count = 10)
    {
        var recentOrders = await _db.Orders.Include(o => o.User).OrderByDescending(o => o.CreatedAt).Take(count).ToListAsync();

        if (recentOrders == null || !recentOrders.Any())
        {
            return new ApiResponse<IEnumerable<RecentOrderDto>>
            {
                Success = false,
                Message = "No se encontraron pedidos recientes.",
                Data = null
            };
        }

        return new ApiResponse<IEnumerable<RecentOrderDto>>
        {
            Success = true,
            Message = "Pedidos recientes obtenidos correctamente.",
            Data =
                recentOrders.Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    UserName = o.User.UserName!,
                    Total = o.Total,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                }).ToList()
        };
    }

    public async Task<ApiResponse<OrdersByStatusDto>> GetOrdersByStatus()
    {
        var groups = await _db.Orders.GroupBy(o => o.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
        var pending = groups.FirstOrDefault(g => g.Status == OrderStatus.Pending)?.Count ?? 0;
        var paid = groups.FirstOrDefault(g => g.Status == OrderStatus.Paid)?.Count ?? 0;
        var cancellled = groups.FirstOrDefault(g => g.Status == OrderStatus.Cancelled)?.Count ?? 0;

        if (pending == 0 && paid == 0 && cancellled == 0)
        {
            return new ApiResponse<OrdersByStatusDto>
            {
                Success = false,
                Message = "No se encontraron pedidos por estado.",
                Data = null
            };
        }


        return new ApiResponse<OrdersByStatusDto>
        {
            Success = true,
            Message = "Pedidos por estado obtenidos correctamente.",
            Data = new OrdersByStatusDto
            {
                Pending = pending,
                Paid = paid,
                Cancelled = cancellled
            }
        };
    }

    public async Task<ApiResponse<IEnumerable<RevenueByMonthDto>>> GetRevenueByMonth(int months = 6)
    {
        var from = DateTime.UtcNow.AddMonths(-months + 1);
        var startDate = new DateTime(from.Year, from.Month, 1);
        var data = await _db.Orders.Where(o => o.Status == OrderStatus.Paid && o.CreatedAt >= startDate).GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(o => o.Total),
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month).ToListAsync();

        // Genera todos los meses aunque no haya datos
        var result = new List<RevenueByMonthDto>();
        for (int i = months - 1; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var found = data.FirstOrDefault(d => d.Year == date.Year && d.Month == date.Month);
            result.Add(new RevenueByMonthDto
            {
                Month = date.ToString("MMM", new System.Globalization.CultureInfo("es-MX")),
                Revenue = found?.Revenue ?? 0,
            });
        }

        return new ApiResponse<IEnumerable<RevenueByMonthDto>>
        {
            Success = true,
            Message = "Ingresos por mes obtenidos correctamente.",
            Data = result
        };
    }
}