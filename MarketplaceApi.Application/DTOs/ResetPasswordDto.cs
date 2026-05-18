using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "El token es requerido")]
    public string Token { get; set; } = null!;
    
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string NewPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = null!;
}