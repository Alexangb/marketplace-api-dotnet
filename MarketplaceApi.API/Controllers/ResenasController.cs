using Microsoft.AspNetCore.Mvc;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;
using MarketplaceApi.Shared.exceptions;
using Microsoft.AspNetCore.Authorization;


namespace MarketplaceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ResenasController : ControllerBase
    {
        private readonly IResenaService _resenaService;

        public ResenasController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        /// <summary>
        /// Obtiene todas las reseñas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ResenaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var resenas = await _resenaService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ResenaDto>>.Ok(resenas));
        }

        /// <summary>
        /// Obtiene una reseña por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ResenaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var resena = await _resenaService.GetByIdAsync(id);
            if (resena == null)
                return NotFound(ApiResponse<ResenaDto>.NotFound($"Reseña {id} no encontrada"));
            
            return Ok(ApiResponse<ResenaDto>.Ok(resena));
        }

        /// <summary>
        /// Obtiene reseñas por servicio
        /// </summary>
        [HttpGet("servicio/{servicioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ResenaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByServicio(int servicioId)
        {
            var resenas = await _resenaService.GetByServicioAsync(servicioId);
            return Ok(ApiResponse<IEnumerable<ResenaDto>>.Ok(resenas));
        }

        /// <summary>
        /// Obtiene reseñas por usuario
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ResenaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            var resenas = await _resenaService.GetByUsuarioAsync(usuarioId);
            return Ok(ApiResponse<IEnumerable<ResenaDto>>.Ok(resenas));
        }

        /// <summary>
        /// Obtiene el promedio de calificación de un servicio
        /// </summary>
        [HttpGet("promedio/{servicioId}")]
        [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPromedio(int servicioId)
        {
            var promedio = await _resenaService.GetPromedioCalificacionAsync(servicioId);
            return Ok(ApiResponse<double>.Ok(promedio));
        }

        /// <summary>
        /// Obtiene la distribución de calificaciones de un servicio
        /// </summary>
        [HttpGet("distribucion/{servicioId}")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<int, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDistribucion(int servicioId)
        {
            var distribucion = await _resenaService.GetDistribucionCalificacionesAsync(servicioId);
            return Ok(ApiResponse<Dictionary<int, int>>.Ok(distribucion));
        }

        /// <summary>
        /// Crea una nueva reseña
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ResenaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] ResenaCreateDto dto)
        {
            try
            {
                var resena = await _resenaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = resena.Id }, 
                    ApiResponse<ResenaDto>.Created(resena, "Reseña creada exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<ResenaDto>.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<ResenaDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza una reseña
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ResenaUpdateDto dto)
        {
            try
            {
                await _resenaService.UpdateAsync(id, dto);
                return Ok(ApiResponse.Ok("Reseña actualizada exitosamente"));
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
        /// Elimina una reseña
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _resenaService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Reseña eliminada exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene reseñas paginadas
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ResenaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? servicioId = null,
            [FromQuery] int? usuarioId = null,
            [FromQuery] int? calificacion = null)
        {
            var filtro = new ResenaFiltroDto
            {
                ServicioId = servicioId,
                UsuarioId = usuarioId,
                Calificacion = calificacion
            };
            
            var result = await _resenaService.GetPagedAsync(page, pageSize, filtro);
            return Ok(ApiResponse<PagedResult<ResenaDto>>.Ok(result));
        }
    }
}