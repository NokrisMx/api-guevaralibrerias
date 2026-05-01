using System;
using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class UpdateBookDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    public string? ImgUrl { get; set; }

    public string? ImgUrlLocal { get; set; }

    public IFormFile? Image { get; set; }

    [Required(ErrorMessage = "El ISBN es obligatorio.")]
    [MaxLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres.")]
    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "El ISBN no tiene un formato válido.")]
    public string ISBN { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "El autor es obligatorio.")]
    public int AuthorId { get; set; }
}
