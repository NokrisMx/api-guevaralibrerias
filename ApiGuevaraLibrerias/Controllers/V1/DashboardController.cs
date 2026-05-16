using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGuevaraLibrerias.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardRepository _repo;

    public DashboardController(IDashboardRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _repo.GetStats();
        return Ok(stats);
    }

    [HttpGet("recent-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRecentOrders([FromQuery] int count = 10)
    {
        var orders = await _repo.GetRecentOrders(count);
        return Ok(orders);
    }
}