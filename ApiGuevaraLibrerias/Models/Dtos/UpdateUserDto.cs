using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class UpdateUserDto
{
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "El username no puede exceder los 50 caracteres.")]
    [MinLength(3, ErrorMessage = "El username debe tener al menos 3 caracteres.")]
    public string? Username { get; set; }

    [EmailAddress(ErrorMessage = "Correo inválido.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Número de teléfono inválido.")]
    public string? PhoneNumber { get; set; }
}