namespace MarketplaceApi.Application.DTOs;

public class CategoriaConConteoDto
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; }
    public int CantidadServicios { get; set; }
    public int CantidadServiciosActivos { get; set; }
    public decimal PrecioPromedio { get; set; }
    public decimal PrecioMinimo { get; set; }
    public decimal PrecioMaximo { get; set; }
    
    // Propiedades calculadas
    public string PrecioPromedioFormateado => PrecioPromedio.ToString("C");
    public string RangoPrecios => $"{PrecioMinimo:C} - {PrecioMaximo:C}";
}