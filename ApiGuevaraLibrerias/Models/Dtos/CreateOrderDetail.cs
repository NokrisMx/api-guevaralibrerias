using System;
using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateOrderDetailDto
{
    [Required(ErrorMessage = "El libro es obligatorio.")]
    public int BookId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Quantity { get; set; }
}
