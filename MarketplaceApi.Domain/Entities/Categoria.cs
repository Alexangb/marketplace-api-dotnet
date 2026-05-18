using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceApi.Domain.Entities;
[Table("categorias")]
public  class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Estado { get; set; } = true;

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
