using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs
{
    public class ReservaDto
    {
         public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? UsuarioEmail { get; set; }
    public int ServicioId { get; set; }
    public string? ServicioNombre { get; set; }
    public decimal? ServicioPrecio { get; set; }
    public int? ServicioDuracion { get; set; }
    public DateOnly FechaReserva { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    
    // Propiedades calculadas
    public string FechaFormateada => FechaReserva.ToString("dd/MM/yyyy");
    public string HoraFormateada => $"{HoraInicio:HH:mm} - {HoraFin:HH:mm}";
    public string EstadoColor => Estado switch
    {
        "Confirmada" => "green",
        "Pendiente" => "orange",
        "Cancelada" => "red",
        "Completada" => "blue",
        _ => "gray"
    };
    public bool EsCancelable => Estado == "Pendiente" || Estado == "Confirmada";
    public decimal PrecioTotal => ServicioPrecio ?? 0;
    }

    public class ReservaCreateDto
    {
      [Required(ErrorMessage = "El ID del cliente es requerido")]
    public int UsuarioId { get; set; }
    
    [Required(ErrorMessage = "El ID del servicio es requerido")]
    public int ServicioId { get; set; }
    
    [Required(ErrorMessage = "La fecha de reserva es requerida")]
    public DateOnly FechaReserva { get; set; }
    
    [Required(ErrorMessage = "La hora de inicio es requerida")]
    public TimeOnly HoraInicio { get; set; }
    
    // Notas opcionales
    public string? Notas { get; set; }
       
    }
}