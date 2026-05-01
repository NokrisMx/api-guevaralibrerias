using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Models;

[Index(nameof(ISBN), IsUnique = true)]
public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? ImgUrl { get; set; }

    public string? ImgUrlLocal { get; set; }

    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$")]
    public string ISBN { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

}
