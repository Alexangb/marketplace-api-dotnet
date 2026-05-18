using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;
[Table("resenas")]
public partial class Resena
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int? ServicioId { get; set; }

    public int? Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Servicio? Servicio { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
