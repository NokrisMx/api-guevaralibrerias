namespace ApiGuevaraLibrerias.Models.Dtos;

public class DashboardStatsDto
{
    public int Books { get; set; }
    public int Authors { get; set; }
    public int Categories { get; set; }
    public int Publishers { get; set; }
    public int Orders { get; set; }
    public int Users { get; set; }
    public decimal Revenue { get; set; }
}