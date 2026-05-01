using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class CreateAuthorDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La biografia no puede exceder los 500 caracteres.")]
    public string Bio { get; set; } = string.Empty;
}
