using System;

namespace MarketplaceApi.Application.DTOs
{
    public class ResenaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? UsuarioFoto { get; set; }
        public int ServicioId { get; set; }
        public string? ServicioNombre { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
        
        // Propiedades calculadas
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
        public string Estrellas => new string('★', Calificacion) + new string('☆', 5 - Calificacion);
    }
    
    public class ResenaCreateDto
    {
        public int UsuarioId { get; set; }
        public int ServicioId { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
    }
    
    public class ResenaUpdateDto
    {
        public int Id { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
    }
    
    public class ResenaFiltroDto
    {
        public int? ServicioId { get; set; }
        public int? UsuarioId { get; set; }
        public int? Calificacion { get; set; }
    }
}