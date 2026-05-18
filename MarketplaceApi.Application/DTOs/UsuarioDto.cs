using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string Email { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public DateTime? FechaRegistro { get; set; }
    public bool? Estado { get; set; }
    public string? FotoUrl { get; set; }
    
    // Propiedades calculadas
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    public string EstadoTexto => Estado == true ? "Activo" : "Inactivo";
}