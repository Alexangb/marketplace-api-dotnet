using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class UserRegisterDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúñÁÉÍÓÚÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras")]
    public string Nombre { get; set; } = null!;
    
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúñÁÉÍÓÚÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras")]
    public string? Apellido { get; set; }
    
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email es inválido")]
    [StringLength(100)]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula y un número")]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = null!;
    
    public string Rol { get; set; } = "Cliente";
}