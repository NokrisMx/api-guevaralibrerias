using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class AuthorDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public ICollection<BookDto> Books { get; set; } = new List<BookDto>();
}
