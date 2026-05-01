using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class UserRegisterDto
{
    [Required(ErrorMessage = "El username es obligatorio.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public string? Name { get; set; }
}
