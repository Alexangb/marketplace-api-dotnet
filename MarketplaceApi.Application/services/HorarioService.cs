using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Domain.Entities;
using MarketplaceApi.Domain.Interfaces;
using MarketplaceApi.Shared.constants;
using MarketplaceApi.Shared.exceptions;
using Microsoft.Extensions.Logging;

namespace MarketplaceApi.Application.Services
{
    public class HorarioService : IHorarioService
    {
        private readonly IGenericRepository<HorariosDisponible> _horarioRepo;
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly ILogger<HorarioService> _logger;

        public HorarioService(
            IGenericRepository<HorariosDisponible> horarioRepo,
            IGenericRepository<Usuario> usuarioRepo,
            ILogger<HorarioService> logger)
        {
            _horarioRepo = horarioRepo;
            _usuarioRepo = usuarioRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<HorarioDto>> GetAllAsync()
        {
            try
            {
                var horarios = await _horarioRepo.GetAllAsync(h => h.Usuario);
                return horarios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los horarios");
                throw;
            }
        }

        public async Task<HorarioDto?> GetByIdAsync(int id)
        {
            try
            {
                var horario = await _horarioRepo.GetByIdAsync(id, h => h.Usuario);
                return horario != null ? MapToDto(horario) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horario {HorarioId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<HorarioDto>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var horarios = await _horarioRepo.FindAsync(
                    h => h.UsuarioId == usuarioId,
                    h => h.Usuario
                );
                return horarios.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horarios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<HorarioDto> CreateAsync(HorarioCreateDto dto)
        {
            try
            {
                // Validaciones
                await ValidarHorarioAsync(dto);

                // Verificar si ya existe horario para ese día
                var existe = await _horarioRepo.AnyAsync(h =>
                    h.UsuarioId == dto.UsuarioId &&
                    h.DiaSemana == dto.DiaSemana);

                if (existe)
                    throw new BusinessException($"Ya existe un horario configurado para el día {DiasSemana.GetNombreDia(dto.DiaSemana)}");

                var horario = new HorariosDisponible
                {
                    UsuarioId = dto.UsuarioId,
                    DiaSemana = dto.DiaSemana,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = dto.HoraFin
                };

                await _horarioRepo.AddAsync(horario);
                await _horarioRepo.SaveAsync();

                _logger.LogInformation("Horario creado exitosamente para usuario {UsuarioId}, día {DiaSemana}",
                    dto.UsuarioId, dto.DiaSemana);

                return MapToDto(horario);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al crear horario para usuario {UsuarioId}", dto.UsuarioId);
                throw;
            }
        }

        public async Task UpdateAsync(int id, HorarioUpdateDto dto)
        {
            try
            {
                var horario = await _horarioRepo.GetByIdAsync(id);
                if (horario == null)
                    throw new NotFoundException("Horario", id);

                await ValidarHorarioAsync(new HorarioCreateDto
                {
                    UsuarioId = dto.UsuarioId,
                    DiaSemana = dto.DiaSemana,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = dto.HoraFin
                });

                // Verificar duplicado (excluyendo el actual)
                var existe = await _horarioRepo.AnyAsync(h =>
                    h.UsuarioId == dto.UsuarioId &&
                    h.DiaSemana == dto.DiaSemana &&
                    h.Id != id);

                if (existe)
                    throw new BusinessException($"Ya existe un horario configurado para el día {DiasSemana.GetNombreDia(dto.DiaSemana)}");

                horario.UsuarioId = dto.UsuarioId;
                horario.DiaSemana = dto.DiaSemana;
                horario.HoraInicio = dto.HoraInicio;
                horario.HoraFin = dto.HoraFin;

                _horarioRepo.Update(horario);
                await _horarioRepo.SaveAsync();

                _logger.LogInformation("Horario actualizado exitosamente: {HorarioId}", id);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar horario {HorarioId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var horario = await _horarioRepo.GetByIdAsync(id);
                if (horario == null)
                    throw new NotFoundException("Horario", id);

                _horarioRepo.Delete(horario);
                await _horarioRepo.SaveAsync();

                _logger.LogInformation("Horario eliminado exitosamente: {HorarioId}", id);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar horario {HorarioId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _horarioRepo.AnyAsync(h => h.Id == id);
        }

        public async Task<bool> ExistsByUsuarioAndDiaAsync(int usuarioId, int diaSemana, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _horarioRepo.AnyAsync(h =>
                    h.UsuarioId == usuarioId &&
                    h.DiaSemana == diaSemana &&
                    h.Id != excludeId.Value);
            }
            return await _horarioRepo.AnyAsync(h =>
                h.UsuarioId == usuarioId &&
                h.DiaSemana == diaSemana);
        }

        public async Task<IEnumerable<HorarioDto>> GetByUsuarioWithDetailsAsync(int usuarioId)
        {
            try
            {
                var horarios = await _horarioRepo.FindAsync(
                    h => h.UsuarioId == usuarioId,
                    h => h.Usuario
                );
                return horarios.Select(MapToDto).OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horarios detallados del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task DeleteByUsuarioAndDiaAsync(int usuarioId, int diaSemana)
        {
            try
            {
                var horarios = await _horarioRepo.FindAsync(h =>
                    h.UsuarioId == usuarioId && h.DiaSemana == diaSemana);

                if (horarios.Any())
                {
                    _horarioRepo.DeleteRange(horarios);
                    await _horarioRepo.SaveAsync();
                    _logger.LogInformation("Horarios eliminados para usuario {UsuarioId}, día {DiaSemana}",
                        usuarioId, diaSemana);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar horarios del usuario {UsuarioId}, día {DiaSemana}",
                    usuarioId, diaSemana);
                throw;
            }
        }

        public async Task DeleteAllByUsuarioAsync(int usuarioId)
        {
            try
            {
                var horarios = await _horarioRepo.FindAsync(h => h.UsuarioId == usuarioId);

                if (horarios.Any())
                {
                    _horarioRepo.DeleteRange(horarios);
                    await _horarioRepo.SaveAsync();
                    _logger.LogInformation("Todos los horarios eliminados para usuario {UsuarioId}", usuarioId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar todos los horarios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task CopyHorariosFromUsuarioAsync(int fromUsuarioId, int toUsuarioId)
        {
            try
            {
                // Verificar que ambos usuarios existen
                var fromUsuario = await _usuarioRepo.GetByIdAsync(fromUsuarioId);
                var toUsuario = await _usuarioRepo.GetByIdAsync(toUsuarioId);

                if (fromUsuario == null)
                    throw new NotFoundException("Usuario origen", fromUsuarioId);

                if (toUsuario == null)
                    throw new NotFoundException("Usuario destino", toUsuarioId);

                // Obtener horarios del usuario origen
                var horariosOrigen = await _horarioRepo.FindAsync(h => h.UsuarioId == fromUsuarioId);

                // Eliminar horarios existentes del usuario destino
                await DeleteAllByUsuarioAsync(toUsuarioId);

                // Copiar horarios
                foreach (var horario in horariosOrigen)
                {
                    var nuevoHorario = new HorariosDisponible
                    {
                        UsuarioId = toUsuarioId,
                        DiaSemana = horario.DiaSemana,
                        HoraInicio = horario.HoraInicio,
                        HoraFin = horario.HoraFin
                    };
                    await _horarioRepo.AddAsync(nuevoHorario);
                }

                await _horarioRepo.SaveAsync();
                _logger.LogInformation("Horarios copiados de usuario {FromUsuarioId} a {ToUsuarioId}",
                    fromUsuarioId, toUsuarioId);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al copiar horarios de usuario {FromUsuarioId} a {ToUsuarioId}",
                    fromUsuarioId, toUsuarioId);
                throw;
            }
        }

        public async Task<bool> IsHorarioValidoAsync(int usuarioId, int diaSemana, TimeOnly horaInicio, TimeOnly horaFin)
        {
            try
            {
                // Validar rango de día
                if (!DiasSemana.IsValidDay(diaSemana))
                    return false;

                // Validar que hora inicio sea menor a hora fin
                if (horaInicio >= horaFin)
                    return false;

                // Validar rango de horas (entre 00:00 y 23:59)
                if (horaInicio < TimeOnly.MinValue || horaFin > TimeOnly.MaxValue)
                    return false;

                // Verificar que el usuario existe
                var usuarioExiste = await _usuarioRepo.AnyAsync(u => u.Id == usuarioId);
                if (!usuarioExiste)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar horario");
                return false;
            }
        }

        public async Task<int> GetTotalHorariosByUsuarioAsync(int usuarioId)
        {
            try
            {
                return await _horarioRepo.CountAsync(h => h.UsuarioId == usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de horarios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<Dictionary<int, int>> GetHorariosAgrupadosPorDiaAsync(int usuarioId)
        {
            try
            {
                var horarios = await _horarioRepo.FindAsync(h => h.UsuarioId == usuarioId);
                
                return horarios
                    .GroupBy(h => h.DiaSemana)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agrupar horarios del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        #region Métodos Privados

        private async Task ValidarHorarioAsync(HorarioCreateDto dto)
        {
            // Validar que el usuario existe
            var usuarioExiste = await _usuarioRepo.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
                throw new NotFoundException("Usuario", dto.UsuarioId);

            // Validar día de semana
            if (!DiasSemana.IsValidDay(dto.DiaSemana))
                throw new BusinessException("El día de semana debe ser entre 1 (Lunes) y 7 (Domingo)");

            // Validar que hora inicio sea menor a hora fin
            if (dto.HoraInicio >= dto.HoraFin)
                throw new BusinessException("La hora de inicio debe ser menor a la hora de fin");

            // Validar que el horario no sea demasiado largo (máximo 12 horas)
            var duracion = dto.HoraFin - dto.HoraInicio;
            if (duracion.TotalHours > 12)
                throw new BusinessException("El horario no puede exceder las 12 horas de duración");
        }

        private static HorarioDto MapToDto(HorariosDisponible h)
        {
            return new HorarioDto
            {
                Id = h.Id,
                UsuarioId = h.UsuarioId ?? 0,
                UsuarioNombre = h.Usuario != null 
                    ? $"{h.Usuario.Nombre} {h.Usuario.Apellido}" 
                    : null,
                DiaSemana = h.DiaSemana,
                DiaNombre = DiasSemana.GetNombreDia(h.DiaSemana),
                HoraInicio = h.HoraInicio,
                HoraFin = h.HoraFin
            };
        }

        #endregion
    }
}