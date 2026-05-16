using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IDashboardRepository
{
    Task<ApiResponse<DashboardStatsDto>> GetStats();
    Task<ApiResponse<IEnumerable<RecentOrderDto>>> GetRecentOrders(int count = 10);
}