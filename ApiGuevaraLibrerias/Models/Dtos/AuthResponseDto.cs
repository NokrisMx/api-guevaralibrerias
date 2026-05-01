using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class UserLoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }
}
