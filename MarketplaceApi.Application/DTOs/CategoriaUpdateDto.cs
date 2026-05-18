using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class CategoriaUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    public string Nombre { get; set; } = null!;
    
    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
    public string? Descripcion { get; set; }
    
    [Required(ErrorMessage = "El estado es requerido")]
    public bool Estado { get; set; }
}