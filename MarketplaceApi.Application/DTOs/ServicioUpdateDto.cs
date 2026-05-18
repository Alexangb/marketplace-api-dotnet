using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class ServicioUpdateDto : ServicioCreateDto
{
    [Required(ErrorMessage = "El ID es requerido")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El estado es requerido")]
    public bool Estado { get; set; }
}