
using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
    public string Name { get; set; } = string.Empty;
}
