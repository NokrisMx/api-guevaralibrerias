namespace ApiGuevaraLibrerias.Models.Dtos;

public class RecentOrderDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}