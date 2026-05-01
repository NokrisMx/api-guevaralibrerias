using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class BookDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? ImgUrl { get; set; }

    public IFormFile? Image { get; set; }

    public string ISBN { get; set; } = string.Empty;

    public int Stock { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}
