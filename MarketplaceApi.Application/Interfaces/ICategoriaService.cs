using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;

namespace MarketplaceApi.Application.Interfaces
{
/// <summary>
/// Servicio para gestión de categorías
/// </summary>
public interface ICategoriaService
{
    // Obtener todas las categorías
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    
    // Obtener solo categorías activas
    Task<IEnumerable<CategoriaDto>> GetActiveAsync();
    
    // Obtener por ID
    Task<CategoriaDto?> GetByIdAsync(int id);
    
    // Crear nueva categoría
    Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto);
    
    // Actualizar categoría existente
    Task UpdateAsync(int id, CategoriaUpdateDto dto); // ✅ Cambiado a UpdateDto
    
    // Eliminar (soft delete)
    Task DeleteAsync(int id);
    
    // ✅ NUEVOS MÉTODOS ÚTILES:
    
    // Obtener con paginación
    Task<PagedResult<CategoriaDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    
    // Verificar si existe
    Task<bool> ExistsAsync(int id);
    
    // Verificar si nombre ya existe
    Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null);
    
    // Activar/Desactivar categoría
    Task ToggleStatusAsync(int id, bool estado);
    
    // Obtener contador de servicios por categoría
    Task<int> GetServiciosCountAsync(int id);
    
    // Obtener categorías con conteo de servicios
    Task<IEnumerable<CategoriaConConteoDto>> GetCategoriasWithServiciosCountAsync();
}
}