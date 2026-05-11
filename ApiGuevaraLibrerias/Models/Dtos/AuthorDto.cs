using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
