using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Models;

[Index(nameof(OrderId))]
[Index(nameof(BookId))]
public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]

    public decimal Price { get; set; }

    //Relaciones
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}
