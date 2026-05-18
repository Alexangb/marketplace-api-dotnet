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
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        /// <summary>
        /// Obtiene todas las reservas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _reservaService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ReservaDto>>.Ok(reservas));
        }

        /// <summary>
        /// Obtiene una reserva por su ID
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _reservaService.GetByIdAsync(id);
            
            if (reserva == null)
                return NotFound(ApiResponse<ReservaDto>.NotFound($"Reserva con ID {id} no encontrada"));
            
            return Ok(ApiResponse<ReservaDto>.Ok(reserva));
        }

        /// <summary>
        /// Obtiene reservas por usuario (cliente)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(int usuarioId)
        {
            var reservas = await _reservaService.GetByUsuarioAsync(usuarioId);
            return Ok(ApiResponse<IEnumerable<ReservaDto>>.Ok(reservas));
        }

        /// <summary>
        /// Obtiene reservas por servicio
        /// </summary>
        /// <param name="servicioId">ID del servicio</param>
        [HttpGet("servicio/{servicioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByServicio(int servicioId)
        {
            var reservas = await _reservaService.GetByServicioAsync(servicioId);
            return Ok(ApiResponse<IEnumerable<ReservaDto>>.Ok(reservas));
        }

        /// <summary>
        /// Obtiene reservas por fecha específica
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="fecha">Fecha (formato: yyyy-MM-dd)</param>
        [HttpGet("usuario/{usuarioId}/fecha")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFecha(int usuarioId, [FromQuery] DateOnly fecha)
        {
            var reservas = await _reservaService.GetReservasByFechaAsync(usuarioId, fecha);
            return Ok(ApiResponse<IEnumerable<ReservaDto>>.Ok(reservas));
        }

        /// <summary>
        /// Obtiene reservas por rango de fechas
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="fechaInicio">Fecha inicio</param>
        /// <param name="fechaFin">Fecha fin</param>
        [HttpGet("usuario/{usuarioId}/rango")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPorRangoFechas(
            int usuarioId,
            [FromQuery] DateOnly fechaInicio,
            [FromQuery] DateOnly fechaFin)
        {
            var reservas = await _reservaService.GetReservasPorRangoFechasAsync(usuarioId, fechaInicio, fechaFin);
            return Ok(ApiResponse<IEnumerable<ReservaDto>>.Ok(reservas));
        }

        /// <summary>
        /// Crea una nueva reserva
        /// </summary>
        /// <param name="dto">Datos de la reserva</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ReservaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] ReservaCreateDto dto)
        {
            try
            {
                var reserva = await _reservaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, 
                    ApiResponse<ReservaDto>.Created(reserva, "Reserva creada exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<ReservaDto>.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<ReservaDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza una reserva existente
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        /// <param name="dto">Datos actualizados</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ReservaUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(ApiResponse.Error("El ID de la ruta no coincide con el ID del cuerpo"));

                await _reservaService.UpdateAsync(id, dto);
                return Ok(ApiResponse.Ok("Reserva actualizada exitosamente"));
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
        /// Elimina una reserva (solo si está cancelada o pendiente)
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _reservaService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Reserva eliminada exitosamente"));
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
        /// Cancela una reserva
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        /// <param name="motivo">Motivo de la cancelación (opcional)</param>
        [HttpPatch("{id}/cancelar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancelar(int id, [FromQuery] string? motivo = null)
        {
            try
            {
                var resultado = await _reservaService.CancelarReservaAsync(id, motivo);
                return Ok(ApiResponse.Ok($"Reserva {id} cancelada exitosamente{(motivo != null ? $" Motivo: {motivo}" : "")}"));
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
        /// Confirma una reserva (cambia de Pendiente a Confirmada)
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        [HttpPatch("{id}/confirmar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Confirmar(int id)
        {
            try
            {
                var resultado = await _reservaService.ConfirmarReservaAsync(id);
                return Ok(ApiResponse.Ok($"Reserva {id} confirmada exitosamente"));
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
        /// Completa una reserva (cambia de Confirmada a Completada)
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        [HttpPatch("{id}/completar")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Completar(int id)
        {
            try
            {
                var resultado = await _reservaService.CompletarReservaAsync(id);
                return Ok(ApiResponse.Ok($"Reserva {id} completada exitosamente"));
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
        /// Cambia el estado de una reserva
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        /// <param name="nuevoEstado">Nuevo estado (Pendiente, Confirmada, Cancelada, Completada)</param>
        [HttpPatch("{id}/estado")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
            {
                return BadRequest(ApiResponse.Error("El estado no puede estar vacío"));
            }

            try
            {
                var resultado = await _reservaService.CambiarEstadoAsync(id, nuevoEstado);
                return Ok(ApiResponse.Ok($"Reserva {id} actualizada a estado: {nuevoEstado}"));
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
        /// Verifica si una reserva existe
        /// </summary>
        /// <param name="id">ID de la reserva</param>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _reservaService.ExistsAsync(id);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Verifica si un horario está disponible
        /// </summary>
        /// <param name="servicioId">ID del servicio</param>
        /// <param name="profesionalId">ID del profesional</param>
        /// <param name="fecha">Fecha de la reserva</param>
        /// <param name="horaInicio">Hora de inicio</param>
        [HttpGet("disponibilidad")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsHorarioDisponible(
            [FromQuery] int servicioId,
            [FromQuery] int profesionalId,
            [FromQuery] DateOnly fecha,
            [FromQuery] TimeOnly horaInicio)
        {
            var disponible = await _reservaService.IsHorarioDisponibleAsync(servicioId, profesionalId, fecha, horaInicio);
            return Ok(ApiResponse<bool>.Ok(disponible));
        }

        #region Dashboard y Estadísticas

        /// <summary>
        /// Obtiene el dashboard de un profesional
        /// </summary>
        /// <param name="profesionalId">ID del profesional</param>
        /// <param name="fecha">Fecha específica (opcional)</param>
        [HttpGet("dashboard/profesional/{profesionalId}")]
        [ProducesResponseType(typeof(ApiResponse<DashboardResumenDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardProfesional(int profesionalId, [FromQuery] DateTime? fecha)
        {
            var agenda = await _reservaService.GetAgendaProfesionalAsync(profesionalId, fecha);
            return Ok(ApiResponse<DashboardResumenDto>.Ok(agenda));
        }

        /// <summary>
        /// Obtiene el dashboard de un cliente
        /// </summary>
        /// <param name="clienteId">ID del cliente</param>
        /// <param name="fecha">Fecha específica (opcional)</param>
        [HttpGet("dashboard/cliente/{clienteId}")]
        [ProducesResponseType(typeof(ApiResponse<DashboardResumenDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardCliente(int clienteId, [FromQuery] DateTime? fecha)
        {
            var agenda = await _reservaService.GetAgendaClienteAsync(clienteId, fecha);
            return Ok(ApiResponse<DashboardResumenDto>.Ok(agenda));
        }

        /// <summary>
        /// Obtiene el historial de ganancias de un profesional
        /// </summary>
        /// <param name="profesionalId">ID del profesional</param>
        /// <param name="dias">Número de días hacia atrás</param>
        [HttpGet("historial-ganancias/{profesionalId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GananciaDiariaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistorialGanancias(int profesionalId, [FromQuery] int dias = 7)
        {
            var historial = await _reservaService.GetHistorialGananciasAsync(profesionalId, dias);
            return Ok(ApiResponse<IEnumerable<GananciaDiariaDto>>.Ok(historial));
        }

        /// <summary>
        /// Obtiene el total de ganancias de un profesional
        /// </summary>
        /// <param name="profesionalId">ID del profesional</param>
        /// <param name="fechaInicio">Fecha inicio (opcional)</param>
        /// <param name="fechaFin">Fecha fin (opcional)</param>
        [HttpGet("ganancias/total/{profesionalId}")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalGanancias(
            int profesionalId,
            [FromQuery] DateOnly? fechaInicio,
            [FromQuery] DateOnly? fechaFin)
        {
            var total = await _reservaService.GetTotalGananciasAsync(profesionalId, fechaInicio, fechaFin);
            return Ok(ApiResponse<decimal>.Ok(total));
        }

        /// <summary>
        /// Obtiene las reservas agrupadas por estado
        /// </summary>
        /// <param name="profesionalId">ID del profesional</param>
        [HttpGet("estadisticas/por-estado/{profesionalId}")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReservasPorEstado(int profesionalId)
        {
            var estadisticas = await _reservaService.GetReservasPorEstadoAsync(profesionalId);
            return Ok(ApiResponse<Dictionary<string, int>>.Ok(estadisticas));
        }

        /// <summary>
        /// Obtiene las reservas activas de hoy para un profesional
        /// </summary>
        /// <param name="profesionalId">ID del profesional</param>
        [HttpGet("estadisticas/activas-hoy/{profesionalId}")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReservasActivasHoy(int profesionalId)
        {
            var total = await _reservaService.GetReservasActivasHoyAsync(profesionalId);
            return Ok(ApiResponse<int>.Ok(total));
        }

        #endregion

        #region Paginación

        /// <summary>
        /// Obtiene reservas paginadas
        /// </summary>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="search">Término de búsqueda</param>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _reservaService.GetPagedAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResult<ReservaDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene reservas paginadas por usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        [HttpGet("usuario/{usuarioId}/paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ReservaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedByUsuario(
            int usuarioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _reservaService.GetPagedByUsuarioAsync(usuarioId, page, pageSize);
            return Ok(ApiResponse<PagedResult<ReservaDto>>.Ok(result));
        }

        #endregion
    }
}