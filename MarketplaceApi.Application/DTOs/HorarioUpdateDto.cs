using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class HorarioUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int UsuarioId { get; set; }
    
    [Required]
    [Range(1, 7, ErrorMessage = "El día debe ser entre 1 (Lunes) y 7 (Domingo)")]
    public int DiaSemana { get; set; }
    
    [Required]
    public TimeOnly HoraInicio { get; set; }
    
    [Required]
    public TimeOnly HoraFin { get; set; }
}