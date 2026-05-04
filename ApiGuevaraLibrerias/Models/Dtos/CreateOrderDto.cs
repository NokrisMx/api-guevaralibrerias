using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateOrderDto
{
    [Required(ErrorMessage = "Debe incluir al menos un libro.")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un libro.")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}