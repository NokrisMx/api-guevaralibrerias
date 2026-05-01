namespace ApiGuevaraLibrerias.Models.Dtos;

public class OrderDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderDetailDto> Items { get; set; } = new();
}
