using System;
using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateOrderDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe incluir al menos un producto.")]
    [MinLength(1, ErrorMessage = "Debe haber al menos un producto en la orden.")]
    public List<CreateOrderDetailDto> Items { get; set; } = new();
}
