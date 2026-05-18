using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs
{
    public class ServicioFiltroDto
    {
        public string? Nombre { get; set; }
        public int? CategoriaId { get; set; }
        [Range(0,999999.99)]
        public decimal? PrecioMin { get; set; }
        [Range(0,999999.99)]
        public decimal? PrecioMax { get; set; }
        // Nuevo filtro: 0 para Domingo, 1 para Lunes... 6 para Sábado
        [Range(0,6,ErrorMessage ="diaSemana debe ser un valor entre 0 (Domingo) y 6 (Sábado)")]
        public int? DiaSemana { get; set; }

        // Nueva propiedad: Ejemplo "10:30"
        public string? Hora { get; set; }
        // Nueva propiedad para la fecha específica
        public DateOnly? Fecha { get; set; }

        //validacion
        public bool IsValid()=>!(PrecioMin.HasValue&&PrecioMax.HasValue&&PrecioMin>PrecioMax);
    }
}