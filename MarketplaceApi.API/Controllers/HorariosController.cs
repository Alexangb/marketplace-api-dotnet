using Microsoft.AspNetCore.Mvc;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;
using MarketplaceApi.Shared.exceptions;

namespace MarketplaceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HorariosController : ControllerBase
    {
        private readonly IHorarioService _horarioService;

        public HorariosController(IHorarioService horarioService)
        {
            _horarioService = horarioService;
        }

        /// <summary>
        /// Obtiene todos los horarios
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HorarioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var horarios = await _horarioService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<HorarioDto>>.Ok(horarios));
        }

        /// <summary>
        /// Obtiene un horario por su ID
        /// </summary>
        /// <param name="id">ID del horario</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HorarioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var horario = await _horarioService.GetByIdAsync(id);
            
            if (horario == null)
                return NotFound(ApiResponse<HorarioDto>.NotFound($"Horario con ID {id} no encontrado"));
            
            return Ok(ApiResponse<HorarioDto>.Ok(horario));
        }

        /// <summary>
        /// Obtiene horarios por usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HorarioDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            try
            {
                var horarios = await _horarioService.GetByUsuarioAsync(usuarioId);
                return Ok(ApiResponse<IEnumerable<HorarioDto>>.Ok(horarios));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene horarios detallados por usuario (con nombres de días)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("usuario/{usuarioId}/detalles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HorarioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUsuarioWithDetails(int usuarioId)
        {
            var horarios = await _horarioService.GetByUsuarioWithDetailsAsync(usuarioId);
            return Ok(ApiResponse<IEnumerable<HorarioDto>>.Ok(horarios));
        }

        /// <summary>
        /// Crea un nuevo horario
        /// </summary>
        /// <param name="dto">Datos del horario</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<HorarioDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] HorarioCreateDto dto)
        {
            try
            {
                var horario = await _horarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = horario.Id }, 
                    ApiResponse<HorarioDto>.Created(horario, "Horario creado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<HorarioDto>.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<HorarioDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza un horario existente
        /// </summary>
        /// <param name="id">ID del horario</param>
        /// <param name="dto">Datos actualizados</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] HorarioUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(ApiResponse.Error("El ID de la ruta no coincide con el ID del cuerpo"));

                await _horarioService.UpdateAsync(id, dto);
                return Ok(ApiResponse.Ok("Horario actualizado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Elimina un horario
        /// </summary>
        /// <param name="id">ID del horario</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _horarioService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Horario eliminado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Elimina todos los horarios de un usuario para un día específico
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="diaSemana">Día de la semana (1=Lunes, 7=Domingo)</param>
        [HttpDelete("usuario/{usuarioId}/dia/{diaSemana}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByUsuarioAndDia(int usuarioId, int diaSemana)
        {
            await _horarioService.DeleteByUsuarioAndDiaAsync(usuarioId, diaSemana);
            return Ok(ApiResponse.Ok($"Horarios del día {diaSemana} eliminados exitosamente"));
        }

        /// <summary>
        /// Elimina todos los horarios de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpDelete("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAllByUsuario(int usuarioId)
        {
            await _horarioService.DeleteAllByUsuarioAsync(usuarioId);
            return Ok(ApiResponse.Ok("Todos los horarios del usuario han sido eliminados"));
        }

        /// <summary>
        /// Copia horarios de un usuario a otro
        /// </summary>
        /// <param name="fromUsuarioId">Usuario origen</param>
        /// <param name="toUsuarioId">Usuario destino</param>
        [HttpPost("copiar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CopyHorarios(
            [FromQuery] int fromUsuarioId, 
            [FromQuery] int toUsuarioId)
        {
            try
            {
                await _horarioService.CopyHorariosFromUsuarioAsync(fromUsuarioId, toUsuarioId);
                return Ok(ApiResponse.Ok($"Horarios copiados del usuario {fromUsuarioId} al {toUsuarioId}"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Verifica si existe un horario
        /// </summary>
        /// <param name="id">ID del horario</param>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _horarioService.ExistsAsync(id);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Verifica si un usuario tiene horario para un día específico
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="diaSemana">Día de la semana</param>
        [HttpGet("usuario/{usuarioId}/exists/{diaSemana}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByUsuarioAndDia(int usuarioId, int diaSemana)
        {
            var exists = await _horarioService.ExistsByUsuarioAndDiaAsync(usuarioId, diaSemana);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Valida si un horario es válido
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="diaSemana">Día de la semana</param>
        /// <param name="horaInicio">Hora de inicio</param>
        /// <param name="horaFin">Hora de fin</param>
        [HttpGet("validar")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsHorarioValido(
            [FromQuery] int usuarioId,
            [FromQuery] int diaSemana,
            [FromQuery] string horaInicio,
            [FromQuery] string horaFin)
        {
            if (!TimeOnly.TryParse(horaInicio, out TimeOnly horaInicioTime) ||
                !TimeOnly.TryParse(horaFin, out TimeOnly horaFinTime))
            {
                return BadRequest(ApiResponse<bool>.Error("Formato de hora inválido. Use HH:mm"));
            }

            var isValid = await _horarioService.IsHorarioValidoAsync(usuarioId, diaSemana, horaInicioTime, horaFinTime);
            return Ok(ApiResponse<bool>.Ok(isValid));
        }

        #region Estadísticas

        /// <summary>
        /// Obtiene el total de horarios por usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("estadisticas/total/usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalHorariosByUsuario(int usuarioId)
        {
            var total = await _horarioService.GetTotalHorariosByUsuarioAsync(usuarioId);
            return Ok(ApiResponse<int>.Ok(total));
        }

        /// <summary>
        /// Obtiene horarios agrupados por día
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("estadisticas/agrupados/usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<int, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHorariosAgrupadosPorDia(int usuarioId)
        {
            var agrupados = await _horarioService.GetHorariosAgrupadosPorDiaAsync(usuarioId);
            return Ok(ApiResponse<Dictionary<int, int>>.Ok(agrupados));
        }

        #endregion
    }
}