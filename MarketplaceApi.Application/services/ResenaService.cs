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
    public class ResenaService : IResenaService
    {
        private readonly IGenericRepository<Resena> _resenaRepo;
        private readonly IGenericRepository<Servicio> _servicioRepo;
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IGenericRepository<Reserva> _reservaRepo;
        private readonly ILogger<ResenaService> _logger;

        public ResenaService(
            IGenericRepository<Resena> resenaRepo,
            IGenericRepository<Servicio> servicioRepo,
            IGenericRepository<Usuario> usuarioRepo,
            IGenericRepository<Reserva> reservaRepo,
            ILogger<ResenaService> logger)
        {
            _resenaRepo = resenaRepo;
            _servicioRepo = servicioRepo;
            _usuarioRepo = usuarioRepo;
            _reservaRepo = reservaRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<ResenaDto>> GetAllAsync()
        {
            var resenas = await _resenaRepo.GetAllAsync(r => r.Usuario, r => r.Servicio);
            return resenas.Select(MapToDto);
        }

        public async Task<ResenaDto?> GetByIdAsync(int id)
        {
            var resena = await _resenaRepo.GetByIdAsync(id, r => r.Usuario, r => r.Servicio);
            return resena != null ? MapToDto(resena) : null;
        }

        public async Task<IEnumerable<ResenaDto>> GetByServicioAsync(int servicioId)
        {
            var resenas = await _resenaRepo.FindAsync(
                r => r.ServicioId == servicioId,
                r => r.Usuario,
                r => r.Servicio
            );
            return resenas.OrderByDescending(r => r.Fecha).Select(MapToDto);
        }

        public async Task<IEnumerable<ResenaDto>> GetByUsuarioAsync(int usuarioId)
        {
            var resenas = await _resenaRepo.FindAsync(
                r => r.UsuarioId == usuarioId,
                r => r.Usuario,
                r => r.Servicio
            );
            return resenas.OrderByDescending(r => r.Fecha).Select(MapToDto);
        }

        public async Task<PagedResult<ResenaDto>> GetPagedAsync(int pageNumber, int pageSize, ResenaFiltroDto? filtro = null)
        {
            System.Linq.Expressions.Expression<Func<Resena, bool>>? predicate = null;

            if (filtro != null)
            {
                if (filtro.ServicioId.HasValue)
                    predicate = r => r.ServicioId == filtro.ServicioId;
                if (filtro.UsuarioId.HasValue)
                    predicate = r => r.UsuarioId == filtro.UsuarioId;
                if (filtro.Calificacion.HasValue)
                    predicate = r => r.Calificacion == filtro.Calificacion;
            }

            var (resenas, totalCount) = await _resenaRepo.GetPagedWithTotalAsync(pageNumber, pageSize, predicate);

            // Cargar relaciones
            var resenasConRelaciones = new List<ResenaDto>();
            foreach (var r in resenas)
            {
                var resenaCompleta = await _resenaRepo.GetByIdAsync(r.Id, r2 => r2.Usuario, r2 => r2.Servicio);
                if (resenaCompleta != null)
                    resenasConRelaciones.Add(MapToDto(resenaCompleta));
            }

            return new PagedResult<ResenaDto>
            {
                Items = resenasConRelaciones,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ResenaDto> CreateAsync(ResenaCreateDto dto)
        {
            try
            {
                // Validaciones
                if (dto.Calificacion < 1 || dto.Calificacion > 5)
                    throw new BusinessException("La calificación debe ser entre 1 y 5 estrellas");

                // Verificar que el servicio existe
                var servicio = await _servicioRepo.GetByIdAsync(dto.ServicioId);
                if (servicio == null)
                    throw new NotFoundException("Servicio", dto.ServicioId);

                // Verificar que el usuario existe
                var usuario = await _usuarioRepo.GetByIdAsync(dto.UsuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", dto.UsuarioId);

                // Verificar que el usuario ha contratado este servicio (tiene reserva completada)
                var tieneReservaCompletada = await _reservaRepo.AnyAsync(r =>
                    r.UsuarioId == dto.UsuarioId &&
                    r.ServicioId == dto.ServicioId &&
                    r.Estado == EstadosReserva.Completada);

                if (!tieneReservaCompletada)
                    throw new BusinessException("Solo puedes calificar servicios que hayas contratado y completado");

                // Verificar que no ha calificado ya este servicio
                var yaCalifico = await _resenaRepo.AnyAsync(r =>
                    r.UsuarioId == dto.UsuarioId &&
                    r.ServicioId == dto.ServicioId);

                if (yaCalifico)
                    throw new BusinessException("Ya has calificado este servicio");

                var resena = new Resena
                {
                    UsuarioId = dto.UsuarioId,
                    ServicioId = dto.ServicioId,
                    Calificacion = dto.Calificacion,
                    Comentario = dto.Comentario?.Trim(),
                    Fecha = DateTime.UtcNow
                };

                await _resenaRepo.AddAsync(resena);
                await _resenaRepo.SaveAsync();

                _logger.LogInformation("Reseña creada para servicio {ServicioId} por usuario {UsuarioId}",
                    dto.ServicioId, dto.UsuarioId);

                return MapToDto(resena);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al crear reseña");
                throw;
            }
        }

        public async Task UpdateAsync(int id, ResenaUpdateDto dto)
        {
            try
            {
                var resena = await _resenaRepo.GetByIdAsync(id);
                if (resena == null)
                    throw new NotFoundException("Reseña", id);

                if (dto.Calificacion < 1 || dto.Calificacion > 5)
                    throw new BusinessException("La calificación debe ser entre 1 y 5 estrellas");

                resena.Calificacion = dto.Calificacion;
                resena.Comentario = dto.Comentario?.Trim();

                _resenaRepo.Update(resena);
                await _resenaRepo.SaveAsync();

                _logger.LogInformation("Reseña {ResenaId} actualizada", id);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar reseña {ResenaId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var resena = await _resenaRepo.GetByIdAsync(id);
                if (resena == null)
                    throw new NotFoundException("Reseña", id);

                _resenaRepo.Delete(resena);
                await _resenaRepo.SaveAsync();

                _logger.LogInformation("Reseña {ResenaId} eliminada", id);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar reseña {ResenaId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _resenaRepo.AnyAsync(r => r.Id == id);
        }

        public async Task<double> GetPromedioCalificacionAsync(int servicioId)
        {
            var resenas = await _resenaRepo.FindAsync(r => r.ServicioId == servicioId);
            if (!resenas.Any())
                return 0;

            return resenas.Average(r => r.Calificacion ?? 0);
        }

        public async Task<Dictionary<int, int>> GetDistribucionCalificacionesAsync(int servicioId)
        {
            var resenas = await _resenaRepo.FindAsync(r => r.ServicioId == servicioId);

            var distribucion = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                distribucion[i] = resenas.Count(r => r.Calificacion == i);
            }

            return distribucion;
        }

        #region Mapeo Privado

        private static ResenaDto MapToDto(Resena r)
        {
            return new ResenaDto
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId ?? 0,
                UsuarioNombre = r.Usuario != null
                    ? $"{r.Usuario.Nombre} {r.Usuario.Apellido}".Trim()
                    : null,
                UsuarioFoto = r.Usuario?.FotoUrl,
                ServicioId = r.ServicioId ?? 0,
                ServicioNombre = r.Servicio?.Nombre,
                Calificacion = r.Calificacion ?? 0,
                Comentario = r.Comentario,
                Fecha = r.Fecha ?? DateTime.UtcNow
            };
        }



        #endregion
    }
}