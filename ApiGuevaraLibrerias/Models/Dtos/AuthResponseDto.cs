using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }
    public string Role { get; set; } = string.Empty;
}
