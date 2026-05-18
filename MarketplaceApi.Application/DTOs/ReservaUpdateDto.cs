using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class ReservaUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int UsuarioId { get; set; }
    
    [Required]
    public int ServicioId { get; set; }
    
    [Required]
    public DateOnly FechaReserva { get; set; }
    
    [Required]
    public TimeOnly HoraInicio { get; set; }
    
    [Required]
    public TimeOnly HoraFin { get; set; }
    
    [Required]
    [RegularExpression("^(Pendiente|Confirmada|Cancelada|Completada)$", 
        ErrorMessage = "Estado no válido")]
    public string Estado { get; set; } = null!;
}