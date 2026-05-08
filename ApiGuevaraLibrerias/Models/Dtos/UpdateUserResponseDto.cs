using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class UpdateUserResponseDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public UserDto? User { get; set; }
}
