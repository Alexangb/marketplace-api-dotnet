using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;

namespace MarketplaceApi.Application.Interfaces
{
    /// <summary>
    /// Servicio para gestión de horarios disponibles de prestadores
    /// </summary>
    public interface IHorarioService
    {
        // Métodos básicos
        Task<IEnumerable<HorarioDto>> GetAllAsync();
        Task<HorarioDto?> GetByIdAsync(int id);
        Task<IEnumerable<HorarioDto>> GetByUsuarioAsync(int usuarioId);
        Task<HorarioDto> CreateAsync(HorarioCreateDto dto);
        Task UpdateAsync(int id, HorarioUpdateDto dto);
        Task DeleteAsync(int id);
        
        // Validaciones
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByUsuarioAndDiaAsync(int usuarioId, int diaSemana, int? excludeId = null);
        
        // Operaciones por usuario
        Task<IEnumerable<HorarioDto>> GetByUsuarioWithDetailsAsync(int usuarioId);
        Task DeleteByUsuarioAndDiaAsync(int usuarioId, int diaSemana);
        Task DeleteAllByUsuarioAsync(int usuarioId);
        
        // Copiar horarios de una semana
        Task CopyHorariosFromUsuarioAsync(int fromUsuarioId, int toUsuarioId);
        
        // Validar horario
        Task<bool> IsHorarioValidoAsync(int usuarioId, int diaSemana, TimeOnly horaInicio, TimeOnly horaFin);
        
        // Dashboard
        Task<int> GetTotalHorariosByUsuarioAsync(int usuarioId);
        Task<Dictionary<int, int>> GetHorariosAgrupadosPorDiaAsync(int usuarioId);
    }
}