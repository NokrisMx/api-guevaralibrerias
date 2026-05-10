using System.Security.Claims;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
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

        var response = await _orderRepository.GetOrders();

        if (!response.Success)
            return BadRequest(response);

        // Filtrar órdenes si no es admin
        if (!isAdmin)
        {
            response.Data = response.Data!
                .Where(o => o.UserId == userId);
        }

        return Ok(response);
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
        {
            return BadRequest(new ApiResponse<OrderDto>
            {
                Success = false,
                Message = "El ID debe ser mayor que 0.",
                Data = null
            });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var response = await _orderRepository.GetOrder(id);

        if (!response.Success)
            return NotFound(response);

        // Validar permisos
        if (!isAdmin && response.Data!.UserId != userId)
            return Forbid();

        return Ok(response);
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse<OrderDto>
            {
                Success = false,
                Message = "Usuario no autenticado",
                Data = null
            });
        }

        var orderDetails = new List<OrderDetail>();
        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var bookResponse = await _bookRepository.GetBook(item.BookId);

            if (!bookResponse.Success || bookResponse.Data == null)
            {
                return NotFound(new ApiResponse<OrderDto>
                {
                    Success = false,
                    Message = $"El libro con ID {item.BookId} no fue encontrado",
                    Data = null
                });
            }

            var book = bookResponse.Data;

            if (book.Stock < item.Quantity)
            {
                return BadRequest(new ApiResponse<OrderDto>
                {
                    Success = false,
                    Message = $"Stock insuficiente para '{book.Title}'. Stock disponible: {book.Stock}",
                    Data = null
                });
            }

            var subtotal = book.Price * item.Quantity;

            total += subtotal;

            orderDetails.Add(new OrderDetail
            {
                BookId = book.Id,
                Quantity = item.Quantity,
                Price = book.Price
            });

            await _bookRepository.UpdateStock(
                book.Id,
                book.Stock - item.Quantity
            );
        }

        var order = new Order
        {
            UserId = userId,
            Total = total,
            Status = OrderStatus.Pending,
            OrderDetails = orderDetails
        };

        var response = await _orderRepository.CreateOrder(order);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}