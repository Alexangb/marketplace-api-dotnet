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
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        /// <summary>
        /// Obtiene todos los servicios activos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var servicios = await _servicioService.GetAllActiveAsync();
            return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(servicios));
        }

        /// <summary>
        /// Obtiene todos los servicios (incluyendo inactivos)
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllIncludingInactive()
        {
            var servicios = await _servicioService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(servicios));
        }

        /// <summary>
        /// Obtiene un servicio por su ID
        /// </summary>
        /// <param name="id">ID del servicio</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ServicioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var servicio = await _servicioService.GetByIdAsync(id);
            
            if (servicio == null)
                return NotFound(ApiResponse<ServicioDto>.NotFound($"Servicio con ID {id} no encontrado"));
            
            return Ok(ApiResponse<ServicioDto>.Ok(servicio));
        }

        /// <summary>
        /// Crea un nuevo servicio
        /// </summary>
        /// <param name="dto">Datos del servicio</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ServicioDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] ServicioCreateDto dto)
        {
            try
            {
                var servicio = await _servicioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = servicio.Id }, 
                    ApiResponse<ServicioDto>.Created(servicio, "Servicio creado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<ServicioDto>.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<ServicioDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza un servicio existente
        /// </summary>
        /// <param name="id">ID del servicio</param>
        /// <param name="dto">Datos actualizados</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ServicioUpdateDto dto)
        {
            try
            {
                await _servicioService.UpdateAsync(id, dto);
                return Ok(ApiResponse.Ok("Servicio actualizado exitosamente"));
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
        /// Elimina un servicio
        /// </summary>
        /// <param name="id">ID del servicio</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _servicioService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Servicio eliminado exitosamente"));
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
        /// Búsqueda avanzada de servicios
        /// </summary>
        /// <param name="filtros">Filtros de búsqueda</param>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Buscar([FromQuery] ServicioFiltroDto filtros)
        {
            var resultados = await _servicioService.BuscarAvanzadoAsync(filtros);
            return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(resultados));
        }

        /// <summary>
        /// Obtiene servicios por categoría
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        [HttpGet("categoria/{categoriaId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategoria(int categoriaId)
        {
            var servicios = await _servicioService.GetByCategoriaAsync(categoriaId);
            return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(servicios));
        }

        /// <summary>
        /// Obtiene servicios por prestador
        /// </summary>
        /// <param name="usuarioId">ID del prestador</param>
        [HttpGet("prestador/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPrestador(int usuarioId)
        {
            var servicios = await _servicioService.GetByPrestadorAsync(usuarioId);
            return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(servicios));
        }

        /// <summary>
        /// Obtiene servicios por usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServicioDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            try
            {
                var servicios = await _servicioService.GetByUsuarioAsync(usuarioId);
                return Ok(ApiResponse<IEnumerable<ServicioDto>>.Ok(servicios));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Activa o desactiva un servicio
        /// </summary>
        /// <param name="id">ID del servicio</param>
        /// <param name="estado">true = activar, false = desactivar</param>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleStatus(int id, [FromQuery] bool estado)
        {
            try
            {
                await _servicioService.ToggleStatusAsync(id, estado);
                var mensaje = estado ? "Servicio activado exitosamente" : "Servicio desactivado exitosamente";
                return Ok(ApiResponse.Ok(mensaje));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene servicios paginados
        /// </summary>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="search">Término de búsqueda</param>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _servicioService.GetPagedAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResult<ServicioDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene servicios paginados por categoría
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        [HttpGet("categoria/{categoriaId}/paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ServicioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedByCategoria(
            int categoriaId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _servicioService.GetPagedByCategoriaAsync(categoriaId, page, pageSize);
            return Ok(ApiResponse<PagedResult<ServicioDto>>.Ok(result));
        }

        /// <summary>
        /// Verifica si un servicio existe
        /// </summary>
        /// <param name="id">ID del servicio</param>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _servicioService.ExistsAsync(id);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Verifica disponibilidad de un servicio en fecha y hora específica
        /// </summary>
        /// <param name="servicioId">ID del servicio</param>
        /// <param name="fecha">Fecha de reserva</param>
        /// <param name="hora">Hora de reserva</param>
        [HttpGet("{servicioId}/disponibilidad")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsDisponible(
            int servicioId,
            [FromQuery] DateOnly fecha,
            [FromQuery] TimeOnly hora)
        {
            var disponible = await _servicioService.IsDisponibleAsync(servicioId, fecha, hora);
            return Ok(ApiResponse<bool>.Ok(disponible));
        }

        #region Estadísticas

        /// <summary>
        /// Obtiene el total de servicios activos
        /// </summary>
        [HttpGet("estadisticas/total")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalServicios()
        {
            var total = await _servicioService.GetTotalServiciosAsync();
            return Ok(ApiResponse<int>.Ok(total));
        }

        /// <summary>
        /// Obtiene el total de servicios por usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        [HttpGet("estadisticas/total/usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTotalServiciosByUsuario(int usuarioId)
        {
            try
            {
                var total = await _servicioService.GetTotalServiciosByUsuarioAsync(usuarioId);
                return Ok(ApiResponse<int>.Ok(total));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<int>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene el precio promedio de todos los servicios
        /// </summary>
        [HttpGet("estadisticas/precio-promedio")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPrecioPromedio()
        {
            var promedio = await _servicioService.GetPrecioPromedioAsync();
            return Ok(ApiResponse<decimal>.Ok(promedio));
        }

        #endregion
    }
}