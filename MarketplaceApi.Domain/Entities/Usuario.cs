using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;

[Table("usuarios")]
public partial class Usuario
{
    public int Id { get; set; }
    // Agregar campo para base64
    public string Nombre { get; set; } = null!;

    public string? Apellido { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public bool? Estado { get; set; }
    public string? FotoUrl { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Biografia { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    public virtual ICollection<HorariosDisponible> HorariosDisponibles { get; set; } = new List<HorariosDisponible>();

    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
