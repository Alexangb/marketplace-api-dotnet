using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Application.DTOs;

public class ServicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Propiedades calculadas
    public string PrecioFormateado => Precio.ToString("C");
    public string DuracionFormateada => DuracionMinutos >= 60
        ? $"{DuracionMinutos / 60}h {(DuracionMinutos % 60 > 0 ? $"{DuracionMinutos % 60}min" : "")}"
        : $"{DuracionMinutos} min";
    public decimal PrecioPorHora => (Precio / DuracionMinutos) * 60;
    

    
}