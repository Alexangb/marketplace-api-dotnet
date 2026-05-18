using System.ComponentModel.DataAnnotations;

namespace MarketplaceApi.Application.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, MinimumLength = 3)]
        public string Nombre { get; set; } = null!;
        public bool Estado { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }

    public class CategoriaCreateDto : CategoriaDto
    {
        [Required]
        public int Id { get; set; }
    }
}