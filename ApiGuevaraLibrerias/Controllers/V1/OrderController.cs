using System.Security.Claims;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGuevaraLibrerias.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBookRepository _bookRepository;

    public OrderController(IOrderRepository orderRepository, IBookRepository bookRepository)
    {
        _orderRepository = orderRepository;
        _bookRepository = bookRepository;
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var orders = await _orderRepository.GetOrders();

        // Si no es admin, filtra solo sus órdenes
        if (!isAdmin)
            orders = orders.Where(o => o.UserId == userId);

        return Ok(orders.Select(o => MapOrderToDto(o)));
    }

    [Authorize]
    [HttpGet("{id:int}", Name = "GetOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor que 0");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var order = await _orderRepository.GetOrder(id);
        if (order == null)
            return NotFound($"La orden con ID {id} no fue encontrada");

        // Si no es admin solo puede ver sus propias órdenes
        if (!isAdmin && order.UserId != userId)
            return Forbid();

        return Ok(MapOrderToDto(order));
    }

    [Authorize]
    [HttpPost("buy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuyBook([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Obtener el ID del usuario autenticado desde el token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Usuario no autenticado");

        var orderDetails = new List<OrderDetail>();
        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var book = await _bookRepository.GetBook(item.BookId);
            if (book == null)
                return NotFound($"El libro con ID {item.BookId} no fue encontrado");

            if (book.Stock < item.Quantity)
                return BadRequest($"Stock insuficiente para '{book.Title}'. Stock disponible: {book.Stock}");

            var subtotal = book.Price * item.Quantity;
            total += subtotal;

            orderDetails.Add(new OrderDetail
            {
                BookId = book.Id,
                Quantity = item.Quantity,
                Price = book.Price
            });

            await _bookRepository.UpdateStock(book.Id, book.Stock - item.Quantity);
        }

        var order = new Order
        {
            UserId = userId,
            Total = total,
            Status = OrderStatus.Pending,
            OrderDetails = orderDetails
        };

        var created = await _orderRepository.CreateOrder(order);
        var orderWithRelations = await _orderRepository.GetOrder(created.Id);

        return StatusCode(StatusCodes.Status201Created, MapOrderToDto(orderWithRelations!));
    }

    // Método privado para mapear Order a OrderDto
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