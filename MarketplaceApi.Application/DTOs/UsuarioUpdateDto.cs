using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class UsuarioUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
    public string Nombre { get; set; } = null!;
    
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
    public string? Apellido { get; set; }
    
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = null!;
}