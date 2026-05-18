using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;  // ✅ Corregido

namespace MarketplaceApi.Application.Interfaces
{
    /// <summary>
    /// Servicio para gestión de servicios
    /// </summary>
    public interface IServicioService
    {
        // Métodos básicos
        Task<IEnumerable<ServicioDto>> GetAllAsync();
        Task<IEnumerable<ServicioDto>> GetAllActiveAsync();
        Task<ServicioDto?> GetByIdAsync(int id);
        Task<ServicioDto> CreateAsync(ServicioCreateDto dto);
        Task UpdateAsync(int id, ServicioUpdateDto dto);
        Task DeleteAsync(int id);
        
        // Búsqueda y filtros
        Task<IEnumerable<ServicioDto>> BuscarAvanzadoAsync(ServicioFiltroDto filtros);
        Task<IEnumerable<ServicioDto>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<ServicioDto>> GetByPrestadorAsync(int usuarioId);
        
        // Paginación
        Task<PagedResult<ServicioDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PagedResult<ServicioDto>> GetPagedByCategoriaAsync(int categoriaId, int pageNumber, int pageSize);
        
        // Validaciones
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null);
        Task<bool> IsDisponibleAsync(int servicioId, DateOnly fecha, TimeOnly hora);
        
        // Estado y gestión
        Task ToggleStatusAsync(int id, bool estado);
        Task<IEnumerable<ServicioDto>> GetByUsuarioAsync(int usuarioId);
        
        // Dashboard y estadísticas
        Task<int> GetTotalServiciosAsync();
        Task<int> GetTotalServiciosByUsuarioAsync(int usuarioId);
        Task<decimal> GetPrecioPromedioAsync();
    }
}