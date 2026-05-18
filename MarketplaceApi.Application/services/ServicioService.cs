using System;
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
    public class ServicioService : IServicioService
    {
        private readonly IGenericRepository<Servicio> _repository;
        private readonly IGenericRepository<HorariosDisponible> _horarioRepository;
        private readonly IGenericRepository<Reserva> _reservaRepository;
        private readonly IGenericRepository<Categoria> _categoriaRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly ILogger<ServicioService> _logger;

        public ServicioService(
            IGenericRepository<Servicio> repository,
            IGenericRepository<HorariosDisponible> horarioRepository,
            IGenericRepository<Reserva> reservaRepository,
            IGenericRepository<Categoria> categoriaRepository,
            IGenericRepository<Usuario> usuarioRepository,
            ILogger<ServicioService> logger)
        {
            _repository = repository;
            _horarioRepository = horarioRepository;
            _reservaRepository = reservaRepository;
            _categoriaRepository = categoriaRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ServicioDto>> GetAllAsync()
        {
            try
            {
                var servicios = await _repository.GetAllAsync(s => s.Categoria, s => s.Usuario);
                return servicios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los servicios");
                throw;
            }
        }

        public async Task<IEnumerable<ServicioDto>> GetAllActiveAsync()
        {
            try
            {
                var servicios = await _repository.FindAsync(
                    s => s.Estado == true,
                    s => s.Categoria,
                    s => s.Usuario
                );
                return servicios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios activos");
                throw;
            }
        }

        public async Task<ServicioDto?> GetByIdAsync(int id)
        {
            try
            {
                var servicios = await _repository.FindAsync(
                    s => s.Id == id,
                    s => s.Categoria,
                    s => s.Usuario
                );
                var servicio = servicios.FirstOrDefault();
                return servicio != null ? MapToDto(servicio) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicio {ServicioId}", id);
                throw;
            }
        }

        public async Task<ServicioDto> CreateAsync(ServicioCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new BusinessException("El nombre del servicio es requerido");

                if (dto.Precio <= 0)
                    throw new BusinessException("El precio debe ser mayor a 0");

                if (dto.DuracionMinutos <= 0)
                    throw new BusinessException("La duración debe ser mayor a 0 minutos");

                var categoriaExiste = await _categoriaRepository.AnyAsync(c => c.Id == dto.CategoriaId);
                if (!categoriaExiste)
                    throw new NotFoundException("Categoría", dto.CategoriaId);

                var usuarioExiste = await _usuarioRepository.AnyAsync(u => u.Id == dto.UsuarioId);
                if (!usuarioExiste)
                    throw new NotFoundException("Usuario", dto.UsuarioId);

                var existeNombre = await _repository.AnyAsync(s =>
                    s.Nombre == dto.Nombre && s.UsuarioId == dto.UsuarioId);
                if (existeNombre)
                    throw new BusinessException("Ya tienes un servicio con ese nombre");

                var nuevo = new Servicio
                {
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    Precio = dto.Precio,
                    DuracionMinutos = dto.DuracionMinutos,
                    CategoriaId = dto.CategoriaId,
                    UsuarioId = dto.UsuarioId,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow
                };

                await _repository.AddAsync(nuevo);
                await _repository.SaveAsync();

                _logger.LogInformation("Servicio creado exitosamente: {ServicioId} - {ServicioNombre}",
                    nuevo.Id, nuevo.Nombre);

                return MapToDto(nuevo);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al crear servicio para usuario {UsuarioId}", dto.UsuarioId);
                throw;
            }
        }

        public async Task UpdateAsync(int id, ServicioUpdateDto dto)
        {
            try
            {
                var servicio = await _repository.GetByIdAsync(id);
                if (servicio == null)
                    throw new NotFoundException("Servicio", id);

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new BusinessException("El nombre del servicio es requerido");

                if (dto.Precio <= 0)
                    throw new BusinessException("El precio debe ser mayor a 0");

                if (dto.DuracionMinutos <= 0)
                    throw new BusinessException("La duración debe ser mayor a 0 minutos");

                var existeNombre = await _repository.AnyAsync(s =>
                    s.Nombre == dto.Nombre && s.UsuarioId == servicio.UsuarioId && s.Id != id);
                if (existeNombre)
                    throw new BusinessException("Ya tienes un servicio con ese nombre");

                servicio.Nombre = dto.Nombre.Trim();
                servicio.Descripcion = dto.Descripcion?.Trim();
                servicio.Precio = dto.Precio;
                servicio.DuracionMinutos = dto.DuracionMinutos;
                servicio.CategoriaId = dto.CategoriaId;
                servicio.Estado = dto.Estado;

                _repository.Update(servicio);
                await _repository.SaveAsync();

                _logger.LogInformation("Servicio actualizado exitosamente: {ServicioId}", id);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar servicio {ServicioId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var servicio = await _repository.GetByIdAsync(id);
                if (servicio == null)
                    throw new NotFoundException("Servicio", id);

                var tieneReservas = await _reservaRepository.AnyAsync(r =>
                    r.ServicioId == id && r.Estado != EstadosReserva.Cancelada);

                if (tieneReservas)
                    throw new BusinessException("No se puede eliminar el servicio porque tiene reservas activas");

                _repository.Delete(servicio);
                await _repository.SaveAsync();

                _logger.LogInformation("Servicio eliminado exitosamente: {ServicioId}", id);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar servicio {ServicioId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ServicioDto>> BuscarAvanzadoAsync(ServicioFiltroDto filtros)
        {
            try
            {
                var servicios = await _repository.GetAllAsync(s => s.Categoria, s => s.Usuario);
                var consulta = servicios.AsQueryable();

                consulta = consulta.Where(s => s.Estado == true);

                if (!string.IsNullOrEmpty(filtros.Nombre))
                {
                    consulta = consulta.Where(s => s.Nombre.Contains(filtros.Nombre, StringComparison.OrdinalIgnoreCase));
                }

                if (filtros.CategoriaId.HasValue)
                {
                    consulta = consulta.Where(s => s.CategoriaId == filtros.CategoriaId);
                }

                if (filtros.PrecioMin.HasValue)
                {
                    consulta = consulta.Where(s => s.Precio >= filtros.PrecioMin);
                }

                if (filtros.PrecioMax.HasValue)
                {
                    consulta = consulta.Where(s => s.Precio <= filtros.PrecioMax);
                }

                if (filtros.Fecha.HasValue && !string.IsNullOrEmpty(filtros.Hora))
                {
                    consulta = await FiltrarPorDisponibilidad(consulta, filtros);
                }

                return consulta.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda avanzada de servicios");
                throw;
            }
        }

        public async Task<IEnumerable<ServicioDto>> GetByCategoriaAsync(int categoriaId)
        {
            try
            {
                var servicios = await _repository.FindAsync(
                    s => s.CategoriaId == categoriaId && s.Estado == true,
                    s => s.Categoria,
                    s => s.Usuario
                );
                return servicios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios por categoría {CategoriaId}", categoriaId);
                throw;
            }
        }

        public async Task<IEnumerable<ServicioDto>> GetByPrestadorAsync(int usuarioId)
        {
            try
            {
                var servicios = await _repository.FindAsync(
                    s => s.UsuarioId == usuarioId,
                    s => s.Categoria,
                    s => s.Usuario
                );
                return servicios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios del prestador {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<ServicioDto>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var usuarioExiste = await _usuarioRepository.AnyAsync(u => u.Id == usuarioId);
                if (!usuarioExiste)
                    throw new NotFoundException("Usuario", usuarioId);

                var servicios = await _repository.FindAsync(
                    s => s.UsuarioId == usuarioId,
                    s => s.Categoria,
                    s => s.Usuario
                );

                return servicios.Select(MapToDto);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al obtener servicios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<PagedResult<ServicioDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                System.Linq.Expressions.Expression<Func<Servicio, bool>>? predicate = null;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    predicate = s => s.Nombre.ToLower().Contains(term) ||
                                    (s.Descripcion != null && s.Descripcion.ToLower().Contains(term));
                }

                var (servicios, totalCount) = await _repository.GetPagedWithTotalAsync(pageNumber, pageSize, predicate);

                return new PagedResult<ServicioDto>
                {
                    Items = servicios.Select(MapToDto),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios paginados");
                throw;
            }
        }

        public async Task<PagedResult<ServicioDto>> GetPagedByCategoriaAsync(int categoriaId, int pageNumber, int pageSize)
        {
            try
            {
                var (servicios, totalCount) = await _repository.GetPagedWithTotalAsync(
                    pageNumber,
                    pageSize,
                    s => s.CategoriaId == categoriaId && s.Estado == true
                );

                return new PagedResult<ServicioDto>
                {
                    Items = servicios.Select(MapToDto),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios por categoría paginados");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            if (excludeId.HasValue)
            {
                return await _repository.AnyAsync(s =>
                    s.Nombre == nombre && s.Id != excludeId.Value);
            }

            return await _repository.AnyAsync(s => s.Nombre == nombre);
        }

        public async Task<bool> IsDisponibleAsync(int servicioId, DateOnly fecha, TimeOnly hora)
        {
            try
            {
                var servicio = await _repository.GetByIdAsync(servicioId);
                if (servicio == null)
                    return false;

                var horaFin = hora.AddMinutes(servicio.DuracionMinutos);

                var tieneReserva = await _reservaRepository.AnyAsync(r =>
                    r.ServicioId == servicioId &&
                    r.FechaReserva == fecha &&
                    r.Estado != EstadosReserva.Cancelada &&
                    ((r.HoraInicio <= hora && r.HoraFin > hora) ||
                     (r.HoraInicio < horaFin && r.HoraFin >= horaFin)));

                return !tieneReserva;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad del servicio {ServicioId}", servicioId);
                throw;
            }
        }

        public async Task ToggleStatusAsync(int id, bool estado)
        {
            try
            {
                var servicio = await _repository.GetByIdAsync(id);
                if (servicio == null)
                    throw new NotFoundException("Servicio", id);

                servicio.Estado = estado;
                _repository.Update(servicio);
                await _repository.SaveAsync();

                _logger.LogInformation("Servicio {Accion}: {ServicioId}",
                    estado ? "activado" : "desactivado", id);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al cambiar estado del servicio {ServicioId}", id);
                throw;
            }
        }

        public async Task<int> GetTotalServiciosAsync()
        {
            try
            {
                return await _repository.CountAsync(s => s.Estado == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de servicios");
                throw;
            }
        }

        public async Task<int> GetTotalServiciosByUsuarioAsync(int usuarioId)
        {
            try
            {
                var usuarioExiste = await _usuarioRepository.AnyAsync(u => u.Id == usuarioId);
                if (!usuarioExiste)
                    throw new NotFoundException("Usuario", usuarioId);

                return await _repository.CountAsync(s => s.UsuarioId == usuarioId && s.Estado == true);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al obtener total de servicios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<decimal> GetPrecioPromedioAsync()
        {
            try
            {
                var servicios = await _repository.FindAsync(s => s.Estado == true);
                if (!servicios.Any()) return 0;
                return servicios.Average(s => s.Precio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular precio promedio de servicios");
                throw;
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Filtra servicios por disponibilidad de horario
        /// </summary>
        private async Task<IQueryable<Servicio>> FiltrarPorDisponibilidad(IQueryable<Servicio> consulta, ServicioFiltroDto filtros)
        {
            // Validar que los datos necesarios estén presentes
            if (!filtros.Fecha.HasValue || string.IsNullOrEmpty(filtros.Hora))
                return consulta;

            // Convertir la hora de string a TimeOnly
            if (!TimeOnly.TryParse(filtros.Hora, out TimeOnly horaBusqueda))
                return consulta;

            // Obtener el día de la semana (1=Lunes, 7=Domingo)
            int diaSemanaBusqueda = GetDiaSemanaNumerico(filtros.Fecha.Value.DayOfWeek);

            // Buscar profesionales que trabajan en ese horario
            var horarios = await _horarioRepository.FindAsync(h =>
                h.DiaSemana == diaSemanaBusqueda &&
                h.HoraInicio <= horaBusqueda &&
                h.HoraFin > horaBusqueda);

            var profesionalesQueTrabajanIds = horarios
                .Where(h => h.UsuarioId.HasValue)
                .Select(h => h.UsuarioId!.Value)
                .Distinct()
                .ToList();

            if (!profesionalesQueTrabajanIds.Any())
                return consulta.Where(s => false);

            // Buscar profesionales ocupados en esa fecha y hora
            var reservas = await _reservaRepository.FindAsync(r =>
                r.FechaReserva == filtros.Fecha.Value &&
                r.HoraInicio == horaBusqueda &&
                r.Estado != EstadosReserva.Cancelada);

            var profesionalesOcupadosIds = reservas
                .Where(r => r.UsuarioId.HasValue)
                .Select(r => r.UsuarioId!.Value)
                .Distinct()
                .ToList();

            // Calcular disponibles (los que trabajan y no están ocupados)
            var disponiblesFinalIds = profesionalesQueTrabajanIds
                .Except(profesionalesOcupadosIds)
                .ToList();

            // Aplicar filtro
            return consulta.Where(s => s.UsuarioId.HasValue && disponiblesFinalIds.Contains(s.UsuarioId.Value));
        }

        private static int GetDiaSemanaNumerico(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => 1
            };
        }

        private static ServicioDto MapToDto(Servicio s)
        {
            return new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Descripcion = s.Descripcion,
                Precio = s.Precio,
                DuracionMinutos = s.DuracionMinutos,
                CategoriaId = s.CategoriaId ?? 0,
                CategoriaNombre = s.Categoria?.Nombre,
                UsuarioId = s.UsuarioId ?? 0,
                UsuarioNombre = s.Usuario != null
                    ? $"{s.Usuario.Nombre} {s.Usuario.Apellido}"
                    : null,
                Estado = s.Estado ?? true
            };
        }

        #endregion
    }
}