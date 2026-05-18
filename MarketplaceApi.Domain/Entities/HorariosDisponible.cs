using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;
[Table("horarios_disponibles")]
public partial class HorariosDisponible
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public int DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
