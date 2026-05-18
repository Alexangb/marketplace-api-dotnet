using Microsoft.AspNetCore.Mvc;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Application.Services;
using MarketplaceApi.Shared.utilities;
using MarketplaceApi.Shared.exceptions;

namespace MarketplaceApi.API.Controllers
{
      [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")] // ✅ Especificar que devuelve JSON
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Obtiene todas las categorías
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoriaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CategoriaDto>>.Ok(categorias));
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            
            if (categoria == null)
                return NotFound(ApiResponse<CategoriaDto>.NotFound($"Categoría con ID {id} no encontrada"));
            
            return Ok(ApiResponse<CategoriaDto>.Ok(categoria));
        }

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        /// <param name="dto">Datos de la categoría</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
        {
            try
            {
                var categoria = await _categoriaService.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = categoria.Id }, 
                    ApiResponse<CategoriaDto>.Created(categoria, "Categoría creada exitosamente")
                );
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<CategoriaDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="dto">Datos actualizados</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateDto dto)
        {
            try
            {
                await _categoriaService.UpdateAsync(id, dto);
                return Ok(ApiResponse.Ok("Categoría actualizada exitosamente"));
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
        /// Elimina una categoría
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoriaService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Categoría eliminada exitosamente"));
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
        /// Obtiene solo categorías activas
        /// </summary>
        [HttpGet("activas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoriaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
        {
            var categorias = await _categoriaService.GetActiveAsync();
            return Ok(ApiResponse<IEnumerable<CategoriaDto>>.Ok(categorias));
        }

        /// <summary>
        /// Obtiene categorías con paginación
        /// </summary>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="search">Término de búsqueda</param>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoriaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _categoriaService.GetPagedAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResult<CategoriaDto>>.Ok(result));
        }

        /// <summary>
        /// Verifica si existe una categoría por nombre
        /// </summary>
        /// <param name="nombre">Nombre a verificar</param>
        /// <param name="excludeId">ID a excluir (para actualizaciones)</param>
        [HttpGet("exists/{nombre}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByName(string nombre, [FromQuery] int? excludeId = null)
        {
            var exists = await _categoriaService.ExistsByNameAsync(nombre, excludeId);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Obtiene el conteo de servicios por categoría
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        [HttpGet("{id}/servicios-count")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiciosCount(int id)
        {
            try
            {
                var count = await _categoriaService.GetServiciosCountAsync(id);
                return Ok(ApiResponse<int>.Ok(count));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<int>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene categorías con conteo de servicios
        /// </summary>
        [HttpGet("with-count")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoriaConConteoDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithServiciosCount()
        {
            var result = await _categoriaService.GetCategoriasWithServiciosCountAsync();
            return Ok(ApiResponse<IEnumerable<CategoriaConConteoDto>>.Ok(result));
        }

        /// <summary>
        /// Activa o desactiva una categoría (Soft Delete)
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="estado">true = activar, false = desactivar</param>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleStatus(int id, [FromQuery] bool estado)
        {
            try
            {
                await _categoriaService.ToggleStatusAsync(id, estado);
                var mensaje = estado ? "Categoría activada exitosamente" : "Categoría desactivada exitosamente";
                return Ok(ApiResponse.Ok(mensaje));
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
    }
}