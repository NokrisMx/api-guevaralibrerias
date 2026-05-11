using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Pages { get; set; }

    public string? ImgUrl { get; set; }

    public string ISBN { get; set; } = string.Empty;

    public int Stock { get; set; }

    public DateTime YearPublished { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;

    public int PublisherId { get; set; }

    public string PublisherName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
