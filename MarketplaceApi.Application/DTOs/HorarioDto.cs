using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs
{
    public class HorarioDto
    {
        public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public int DiaSemana { get; set; }
    public string? DiaNombre { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    
    // Propiedades calculadas
    public string HoraInicioFormateada => HoraInicio.ToString(@"hh\:mm");
    public string HoraFinFormateada => HoraFin.ToString(@"hh\:mm");
    public string RangoHorario => $"{HoraInicioFormateada} - {HoraFinFormateada}";
    public int DuracionMinutos => (int)(HoraFin - HoraInicio).TotalMinutes;
    public string DuracionFormateada => DuracionMinutos >= 60 
        ? $"{DuracionMinutos / 60}h {DuracionMinutos % 60}min"
        : $"{DuracionMinutos}min";
    }

    public class HorarioCreateDto
    {
         [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int UsuarioId { get; set; }
    
    [Required(ErrorMessage = "El día de la semana es requerido")]
    [Range(1, 7, ErrorMessage = "El día debe ser entre 1 (Lunes) y 7 (Domingo)")]
    public int DiaSemana { get; set; }
    
    [Required(ErrorMessage = "La hora de inicio es requerida")]
    public TimeOnly HoraInicio { get; set; }
    
    [Required(ErrorMessage = "La hora de fin es requerida")]
    public TimeOnly HoraFin { get; set; }
    }
}