using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateOrderItemDto
{
    [Required(ErrorMessage = "El libro es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del libro debe ser mayor que 0.")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Quantity { get; set; }
}