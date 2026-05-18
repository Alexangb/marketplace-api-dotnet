using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Domain.Entities;
using MarketplaceApi.Domain.Interfaces;
using MarketplaceApi.Shared.constants;
using MarketplaceApi.Shared.exceptions;
using MarketplaceApi.Shared.utilities;
using Microsoft.Extensions.Logging;

namespace MarketplaceApi.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IGenericRepository<Reserva> _reservaRepo;
        private readonly IGenericRepository<Servicio> _servicioRepo;
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IGenericRepository<HorariosDisponible> _horarioRepo;
        private readonly ILogger<ReservaService> _logger;

        public ReservaService(
            IGenericRepository<Reserva> reservaRepo,
            IGenericRepository<Servicio> servicioRepo,
            IGenericRepository<Usuario> usuarioRepo,
            IGenericRepository<HorariosDisponible> horarioRepo,
            ILogger<ReservaService> logger)
        {
            _reservaRepo = reservaRepo;
            _servicioRepo = servicioRepo;
            _usuarioRepo = usuarioRepo;
            _horarioRepo = horarioRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<ReservaDto>> GetAllAsync()
        {
            try
            {
                var reservas = await _reservaRepo.GetAllAsync(r => r.Servicio, r => r.Usuario);
                return reservas.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las reservas");
                throw;
            }
        }

        public async Task<ReservaDto?> GetByIdAsync(int id)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(id, r => r.Servicio, r => r.Usuario);
                return reserva != null ? MapToDto(reserva) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva {ReservaId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var reservas = await _reservaRepo.FindAsync(
                    r => r.UsuarioId == usuarioId,
                    r => r.Servicio,
                    r => r.Usuario
                );
                return reservas.Select(MapToDto).OrderByDescending(r => r.FechaReserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetByServicioAsync(int servicioId)
        {
            try
            {
                var reservas = await _reservaRepo.FindAsync(
                    r => r.ServicioId == servicioId,
                    r => r.Servicio,
                    r => r.Usuario
                );
                return reservas.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas del servicio {ServicioId}", servicioId);
                throw;
            }
        }

        public async Task<ReservaDto> CreateAsync(ReservaCreateDto dto)
        {
            try
            {
                // Validar que el servicio existe
                var servicio = await _servicioRepo.GetByIdAsync(dto.ServicioId);
                if (servicio == null)
                    throw new NotFoundException("Servicio", dto.ServicioId);

                // Validar que el usuario existe
                var usuario = await _usuarioRepo.GetByIdAsync(dto.UsuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", dto.UsuarioId);

                // Validar que la fecha no sea en el pasado
                if (dto.FechaReserva < DateOnly.FromDateTime(DateTime.Today))
                    throw new BusinessException("No se pueden hacer reservas en fechas pasadas");

                // Calcular hora fin
                var horaFin = dto.HoraInicio.AddMinutes(servicio.DuracionMinutos);

                // Validar disponibilidad del horario
                var disponible = await IsHorarioDisponibleAsync(
                    dto.ServicioId,
                    servicio.UsuarioId ?? 0,
                    dto.FechaReserva,
                    dto.HoraInicio
                );

                if (!disponible)
                    throw new BusinessException("El horario seleccionado no está disponible");

                var reserva = new Reserva
                {
                    UsuarioId = dto.UsuarioId,
                    ServicioId = dto.ServicioId,
                    FechaReserva = dto.FechaReserva,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = horaFin,
                    Estado = EstadosReserva.Pendiente,
                    FechaCreacion = DateTime.UtcNow
                };

                await _reservaRepo.AddAsync(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva creada exitosamente: {ReservaId} para usuario {UsuarioId}",
                    reserva.Id, dto.UsuarioId);

                return MapToDto(reserva);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al crear reserva");
                throw;
            }
        }

        public async Task UpdateAsync(int id, ReservaUpdateDto dto)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(id);
                if (reserva == null)
                    throw new NotFoundException("Reserva", id);

                reserva.UsuarioId = dto.UsuarioId;
                reserva.ServicioId = dto.ServicioId;
                reserva.FechaReserva = dto.FechaReserva;
                reserva.HoraInicio = dto.HoraInicio;
                reserva.HoraFin = dto.HoraFin;
                reserva.Estado = dto.Estado;

                _reservaRepo.Update(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva actualizada exitosamente: {ReservaId}", id);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar reserva {ReservaId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(id);
                if (reserva == null)
                    throw new NotFoundException("Reserva", id);

                // Solo se pueden eliminar reservas canceladas o pendientes
                if (reserva.Estado != EstadosReserva.Cancelada && reserva.Estado != EstadosReserva.Pendiente)
                    throw new BusinessException("No se puede eliminar una reserva confirmada o completada");

                _reservaRepo.Delete(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva eliminada exitosamente: {ReservaId}", id);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar reserva {ReservaId}", id);
                throw;
            }
        }

        public async Task<bool> CancelarReservaAsync(int reservaId, string? motivo = null)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(reservaId);
                if (reserva == null)
                    throw new NotFoundException("Reserva", reservaId);

                if (!EstadosReserva.CanCancel(reserva.Estado))
                    throw new BusinessException($"No se puede cancelar una reserva en estado {reserva.Estado}");

                reserva.Estado = EstadosReserva.Cancelada;
                _reservaRepo.Update(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva cancelada exitosamente: {ReservaId}. Motivo: {Motivo}",
                    reservaId, motivo ?? "No especificado");

                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al cancelar reserva {ReservaId}", reservaId);
                throw;
            }
        }

        public async Task<bool> ConfirmarReservaAsync(int reservaId)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(reservaId);
                if (reserva == null)
                    throw new NotFoundException("Reserva", reservaId);

                if (reserva.Estado != EstadosReserva.Pendiente)
                    throw new BusinessException($"No se puede confirmar una reserva en estado {reserva.Estado}");

                reserva.Estado = EstadosReserva.Confirmada;
                _reservaRepo.Update(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva confirmada exitosamente: {ReservaId}", reservaId);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al confirmar reserva {ReservaId}", reservaId);
                throw;
            }
        }

        public async Task<bool> CompletarReservaAsync(int reservaId)
        {
            try
            {
                var reserva = await _reservaRepo.GetByIdAsync(reservaId);
                if (reserva == null)
                    throw new NotFoundException("Reserva", reservaId);

                if (reserva.Estado != EstadosReserva.Confirmada)
                    throw new BusinessException($"No se puede completar una reserva en estado {reserva.Estado}");

                reserva.Estado = EstadosReserva.Completada;
                _reservaRepo.Update(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Reserva completada exitosamente: {ReservaId}", reservaId);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al completar reserva {ReservaId}", reservaId);
                throw;
            }
        }

        public async Task<bool> CambiarEstadoAsync(int reservaId, string nuevoEstado)
        {
            try
            {
                if (!EstadosReserva.IsValidState(nuevoEstado))
                    throw new BusinessException($"Estado '{nuevoEstado}' no es válido");

                var reserva = await _reservaRepo.GetByIdAsync(reservaId);
                if (reserva == null)
                    throw new NotFoundException("Reserva", reservaId);

                reserva.Estado = nuevoEstado;
                _reservaRepo.Update(reserva);
                await _reservaRepo.SaveAsync();

                _logger.LogInformation("Estado de reserva {ReservaId} cambiado a {NuevoEstado}",
                    reservaId, nuevoEstado);

                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al cambiar estado de reserva {ReservaId}", reservaId);
                throw;
            }
        }

        public async Task<DashboardResumenDto> GetAgendaProfesionalAsync(int profesionalId, DateTime? fecha)
        {
            try
            {
                var fechaObj = fecha ?? DateTime.Today;
                var fechaDateOnly = DateOnly.FromDateTime(fechaObj);

                var reservas = await _reservaRepo.FindAsync(
                    r => r.Servicio != null && r.Servicio.UsuarioId == profesionalId && r.FechaReserva == fechaDateOnly,
                    r => r.Servicio,
                    r => r.Usuario
                );

                var reservasDto = reservas.Select(MapToDashboardDto);
                var ganancias = reservas
                    .Where(r => r.Estado == EstadosReserva.Completada)
                    .Sum(r => r.Servicio?.Precio ?? 0);

                return new DashboardResumenDto
                {
                    TotalCitasHoy = reservas.Count(),
                    CitasCompletadas = reservas.Count(r => r.Estado == EstadosReserva.Completada),
                    GananciasDelDia = ganancias,
                    Reservas = reservasDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener agenda del profesional {ProfesionalId}", profesionalId);
                throw;
            }
        }

        public async Task<DashboardResumenDto> GetAgendaClienteAsync(int clienteId, DateTime? fecha)
        {
            try
            {
                var fechaObj = fecha ?? DateTime.Today;
                var fechaDateOnly = DateOnly.FromDateTime(fechaObj);

                var reservas = await _reservaRepo.FindAsync(
                    r => r.UsuarioId == clienteId && r.FechaReserva == fechaDateOnly,
                    r => r.Servicio,
                    r => r.Usuario
                );

                var reservasDto = reservas.Select(MapToDashboardDto);

                return new DashboardResumenDto
                {
                    TotalCitasHoy = reservas.Count(),
                    CitasCompletadas = reservas.Count(r => r.Estado == EstadosReserva.Completada),
                    Reservas = reservasDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener agenda del cliente {ClienteId}", clienteId);
                throw;
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasByFechaAsync(int usuarioId, DateOnly fecha)
        {
            try
            {
                var reservas = await _reservaRepo.FindAsync(
                    r => r.UsuarioId == usuarioId && r.FechaReserva == fecha,
                    r => r.Servicio,
                    r => r.Usuario
                );
                return reservas.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por fecha");
                throw;
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasPorRangoFechasAsync(int usuarioId, DateOnly fechaInicio, DateOnly fechaFin)
        {
            try
            {
                var reservas = await _reservaRepo.FindAsync(
                    r => r.UsuarioId == usuarioId && r.FechaReserva >= fechaInicio && r.FechaReserva <= fechaFin,
                    r => r.Servicio,
                    r => r.Usuario
                );
                return reservas.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por rango de fechas");
                throw;
            }
        }

        public async Task<IEnumerable<GananciaDiariaDto>> GetHistorialGananciasAsync(int profesionalId, int diasAtras)
        {
            try
            {
                var fechaFin = DateOnly.FromDateTime(DateTime.Today);
                var fechaInicio = fechaFin.AddDays(-diasAtras);

                var reservas = await _reservaRepo.FindAsync(
                    r => r.Servicio != null && 
                         r.Servicio.UsuarioId == profesionalId && 
                         r.FechaReserva >= fechaInicio && 
                         r.FechaReserva <= fechaFin &&
                         r.Estado == EstadosReserva.Completada,
                    r => r.Servicio
                );

                var gananciasPorDia = reservas
                    .GroupBy(r => r.FechaReserva)
                    .Select(g => new GananciaDiariaDto
                    {
                        Fecha = g.Key,
                        TotalGanado = g.Sum(r => r.Servicio?.Precio ?? 0),
                        ServiciosCompletados = g.Count()
                    })
                    .OrderByDescending(g => g.Fecha);

                return gananciasPorDia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de ganancias");
                throw;
            }
        }

        public async Task<decimal> GetTotalGananciasAsync(int profesionalId, DateOnly? fechaInicio = null, DateOnly? fechaFin = null)
        {
            try
            {
                var inicio = fechaInicio ?? DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                var fin = fechaFin ?? DateOnly.FromDateTime(DateTime.Now);

                var reservas = await _reservaRepo.FindAsync(
                    r => r.Servicio != null && 
                         r.Servicio.UsuarioId == profesionalId && 
                         r.FechaReserva >= inicio && 
                         r.FechaReserva <= fin &&
                         r.Estado == EstadosReserva.Completada,
                    r => r.Servicio
                );

                return reservas.Sum(r => r.Servicio?.Precio ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de ganancias");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetReservasPorEstadoAsync(int profesionalId)
        {
            try
            {
                var reservas = await _reservaRepo.FindAsync(
                    r => r.Servicio != null && r.Servicio.UsuarioId == profesionalId
                );

                return reservas
                    .GroupBy(r => r.Estado)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por estado");
                throw;
            }
        }

        public async Task<int> GetReservasActivasHoyAsync(int profesionalId)
        {
            try
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                
                var reservas = await _reservaRepo.FindAsync(
                    r => r.Servicio != null && 
                         r.Servicio.UsuarioId == profesionalId && 
                         r.FechaReserva == hoy &&
                         r.Estado != EstadosReserva.Cancelada
                );

                return reservas.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas activas hoy");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _reservaRepo.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> IsHorarioDisponibleAsync(int servicioId, int profesionalId, DateOnly fecha, TimeOnly horaInicio)
        {
            try
            {
                var servicio = await _servicioRepo.GetByIdAsync(servicioId);
                if (servicio == null || servicio.UsuarioId != profesionalId)
                    return false;

                var horaFin = horaInicio.AddMinutes(servicio.DuracionMinutos);

                // Verificar si hay reservas en ese horario
                var reservaExistente = await _reservaRepo.AnyAsync(r =>
                    r.ServicioId == servicioId &&
                    r.FechaReserva == fecha &&
                    r.Estado != EstadosReserva.Cancelada &&
                    ((r.HoraInicio <= horaInicio && r.HoraFin > horaInicio) ||
                     (r.HoraInicio < horaFin && r.HoraFin >= horaFin) ||
                     (r.HoraInicio >= horaInicio && r.HoraFin <= horaFin)));

                return !reservaExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad de horario");
                return false;
            }
        }

        public async Task<bool> TieneReservasActivasAsync(int servicioId)
        {
            return await _reservaRepo.AnyAsync(r =>
                r.ServicioId == servicioId &&
                r.Estado != EstadosReserva.Cancelada &&
                r.Estado != EstadosReserva.Completada);
        }

        public async Task<PagedResult<ReservaDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var (reservas, totalCount) = await _reservaRepo.GetPagedWithTotalAsync(pageNumber, pageSize);

                return new PagedResult<ReservaDto>
                {
                    Items = reservas.Select(MapToDto),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas paginadas");
                throw;
            }
        }

        public async Task<PagedResult<ReservaDto>> GetPagedByUsuarioAsync(int usuarioId, int pageNumber, int pageSize)
        {
            try
            {
                var (reservas, totalCount) = await _reservaRepo.GetPagedWithTotalAsync(
                    pageNumber,
                    pageSize,
                    r => r.UsuarioId == usuarioId
                );

                return new PagedResult<ReservaDto>
                {
                    Items = reservas.Select(MapToDto),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas paginadas por usuario");
                throw;
            }
        }

        #region Mapeo Privado

        private static ReservaDto MapToDto(Reserva r)
        {
            return new ReservaDto
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId ?? 0,
                UsuarioNombre = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}" : null,
                UsuarioEmail = r.Usuario?.Email,
                ServicioId = r.ServicioId ?? 0,
                ServicioNombre = r.Servicio?.Nombre,
                ServicioPrecio = r.Servicio?.Precio,
                ServicioDuracion = r.Servicio?.DuracionMinutos,
                FechaReserva = r.FechaReserva,
                HoraInicio = r.HoraInicio,
                HoraFin = r.HoraFin,
                Estado = r.Estado ?? EstadosReserva.Pendiente,
                FechaCreacion = r.FechaCreacion ?? DateTime.UtcNow
            };
        }

        private static DashboardReservaDto MapToDashboardDto(Reserva r)
        {
            return new DashboardReservaDto
            {
                ReservaId = r.Id,
                ClienteNombre = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}" : "N/A",
                ServicioNombre = r.Servicio?.Nombre ?? "N/A",
                Hora = $"{r.HoraInicio:HH:mm} - {r.HoraFin:HH:mm}",
                Estado = r.Estado ?? EstadosReserva.Pendiente,
                Notas = null
            };
        }

        #endregion
    }
}