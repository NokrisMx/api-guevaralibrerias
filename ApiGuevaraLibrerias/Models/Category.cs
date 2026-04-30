using System.ComponentModel.DataAnnotations;

namespace ApiGuevaraLibrerias.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
