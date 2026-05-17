namespace ApiGuevaraLibrerias.Models.Dtos;

public class OrdersByStatusDto
{
    public int Pending { get; set; }
    public int Paid { get; set; }
    public int Cancelled { get; set; }
}