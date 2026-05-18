using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;
[Table("servicios")]
public partial class Servicio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int DuracionMinutos { get; set; }

    public int? CategoriaId { get; set; }

    public int? UsuarioId { get; set; }

    public bool? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Usuario? Usuario { get; set; }
    // ✅ NUEVOS CAMPOS - Agrega esto
    public int? Visitas { get; set; }
    public decimal? PrecioDescuento { get; set; }
    public int? MaxReservasPorDia { get; set; }
}
