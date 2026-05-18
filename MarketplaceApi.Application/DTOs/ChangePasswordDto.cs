using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    public string CurrentPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula y un número")]
    public string NewPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = null!;
}