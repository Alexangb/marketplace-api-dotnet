using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;

namespace MarketplaceApi.Application.Interfaces
{
    public interface IResenaService
    {
        Task<IEnumerable<ResenaDto>> GetAllAsync();
        Task<ResenaDto?> GetByIdAsync(int id);
        Task<IEnumerable<ResenaDto>> GetByServicioAsync(int servicioId);
        Task<IEnumerable<ResenaDto>> GetByUsuarioAsync(int usuarioId);
        Task<PagedResult<ResenaDto>> GetPagedAsync(int pageNumber, int pageSize, ResenaFiltroDto? filtro = null);
        Task<ResenaDto> CreateAsync(ResenaCreateDto dto);
        Task UpdateAsync(int id, ResenaUpdateDto dto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<double> GetPromedioCalificacionAsync(int servicioId);
        Task<Dictionary<int, int>> GetDistribucionCalificacionesAsync(int servicioId);
    }
}