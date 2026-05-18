using System;

namespace MarketplaceApi.Application.DTOs
{
    /// <summary>
    /// DTO para mostrar el perfil de un usuario
    /// </summary>
    public class UsuarioPerfilDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; }
        public string Email { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public string Rol { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
        public bool? Estado { get; set; }
        
        // Propiedades calculadas
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
        public string EstadoTexto => Estado == true ? "Activo" : "Inactivo";
        public string RolTexto => Rol switch
        {
            "Admin" => "Administrador",
            "Prestador" => "Prestador de Servicios",
            "Cliente" => "Cliente",
            _ => Rol
        };
        public string FechaRegistroFormateada => FechaRegistro?.ToString("dd/MM/yyyy") ?? "No disponible";
        public string Iniciales => !string.IsNullOrEmpty(NombreCompleto) 
            ? new string(NombreCompleto.Split(' ').Select(s => s[0]).Take(2).ToArray()) 
            : "?";
    }
}