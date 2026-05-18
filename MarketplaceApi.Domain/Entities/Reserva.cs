using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;
[Table("reservas")]
public partial class Reserva
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int? ServicioId { get; set; }

    public DateOnly FechaReserva { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Servicio? Servicio { get; set; }

    public virtual Usuario? Usuario { get; set; }
     public string? Notas { get; set; }
    public string? MotivoCancelacion { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public decimal? PrecioFinal { get; set; }
}
