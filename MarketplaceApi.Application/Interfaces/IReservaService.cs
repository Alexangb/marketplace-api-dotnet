using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;

namespace MarketplaceApi.Application.Interfaces
{
    /// <summary>
    /// Servicio para gestión de reservas
    /// </summary>
    public interface IReservaService
    {
        // Métodos básicos
        Task<IEnumerable<ReservaDto>> GetAllAsync();
        Task<ReservaDto?> GetByIdAsync(int id);
        Task<IEnumerable<ReservaDto>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<ReservaDto>> GetByServicioAsync(int servicioId);
        Task<ReservaDto> CreateAsync(ReservaCreateDto dto);
        Task UpdateAsync(int id, ReservaUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        
        // Gestión de estados
        Task<bool> CancelarReservaAsync(int reservaId, string? motivo = null);
        Task<bool> ConfirmarReservaAsync(int reservaId);
        Task<bool> CompletarReservaAsync(int reservaId);
        Task<bool> CambiarEstadoAsync(int reservaId, string nuevoEstado);
        
        // Dashboard y agenda
        Task<DashboardResumenDto> GetAgendaProfesionalAsync(int profesionalId, DateTime? fecha);
        Task<DashboardResumenDto> GetAgendaClienteAsync(int clienteId, DateTime? fecha);
        Task<IEnumerable<ReservaDto>> GetReservasByFechaAsync(int usuarioId, DateOnly fecha);
        Task<IEnumerable<ReservaDto>> GetReservasPorRangoFechasAsync(int usuarioId, DateOnly fechaInicio, DateOnly fechaFin);
        
        // Estadísticas y ganancias
        Task<IEnumerable<GananciaDiariaDto>> GetHistorialGananciasAsync(int profesionalId, int diasAtras);
        Task<decimal> GetTotalGananciasAsync(int profesionalId, DateOnly? fechaInicio = null, DateOnly? fechaFin = null);
        Task<Dictionary<string, int>> GetReservasPorEstadoAsync(int profesionalId);
        Task<int> GetReservasActivasHoyAsync(int profesionalId);
        
        // Validaciones
        Task<bool> ExistsAsync(int id);
        Task<bool> IsHorarioDisponibleAsync(int servicioId, int profesionalId, DateOnly fecha, TimeOnly horaInicio);
        Task<bool> TieneReservasActivasAsync(int servicioId);
        
        // Paginación
        Task<PagedResult<ReservaDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PagedResult<ReservaDto>> GetPagedByUsuarioAsync(int usuarioId, int pageNumber, int pageSize);
    }
}