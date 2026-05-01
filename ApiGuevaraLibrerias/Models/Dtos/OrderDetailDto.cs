using System;

namespace ApiGuevaraLibrerias.Models.Dtos;

public class OrderDetailDto
{

    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal SubTotal { get; set; }
}
