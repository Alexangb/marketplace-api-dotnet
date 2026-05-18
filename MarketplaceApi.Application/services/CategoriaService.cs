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
    /// <summary>
    /// Servicio para gestión de categorías
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Servicio> _servicioRepository;
        private readonly IGenericRepository<Categoria> _repository;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(
            IGenericRepository<Categoria> repository,
            ILogger<CategoriaService> logger, IGenericRepository<Servicio> servicioRepository)
        {
            _repository = repository;
            _servicioRepository = servicioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            try
            {
                var categorias = await _repository.GetAllAsync();
                return categorias.Select(c => MapToDto(c));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las categorías");
                throw;
            }
        }

        public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessException("El nombre de la categoría es requerido");

            // Verificar si ya existe una categoría con el mismo nombre
            var existe = await _repository.AnyAsync(c => c.Nombre == dto.Nombre);
            if (existe)
                throw new BusinessException(MensajesError.CategoriaYaExiste);

            var nuevaCategoria = new Categoria
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                Estado = true
            };

            await _repository.AddAsync(nuevaCategoria);
            await _repository.SaveAsync();

            _logger.LogInformation("Categoría creada exitosamente: {CategoriaId} - {CategoriaNombre}",
                nuevaCategoria.Id, nuevaCategoria.Nombre);

            return MapToDto(nuevaCategoria);
        }

        public async Task<CategoriaDto?> GetByIdAsync(int id)
        {
            try
            {
                var categoria = await _repository.GetByIdAsync(id);
                return categoria != null ? MapToDto(categoria) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría con ID: {CategoriaId}", id);
                throw;
            }
        }

        public async Task UpdateAsync(int id, CategoriaCreateDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessException("El nombre de la categoría es requerido");

            var existente = await _repository.GetByIdAsync(id);
            if (existente == null)
                throw new NotFoundException("Categoría", id);

            // Verificar si ya existe otra categoría con el mismo nombre
            var existeDuplicado = await _repository.AnyAsync(c => c.Nombre == dto.Nombre && c.Id != id);
            if (existeDuplicado)
                throw new BusinessException(MensajesError.CategoriaYaExiste);

            existente.Nombre = dto.Nombre.Trim();
            existente.Descripcion = dto.Descripcion?.Trim();

            _repository.Update(existente);
            await _repository.SaveAsync();

            _logger.LogInformation("Categoría actualizada exitosamente: {CategoriaId} - {CategoriaNombre}",
                id, dto.Nombre);
        }

        public async Task DeleteAsync(int id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null)
                throw new NotFoundException("Categoría", id);

            // Verificar si la categoría tiene servicios asociados
            // NOTA: Necesitarás inyectar IGenericRepository<Servicio> para esto
            // Por ahora, asumimos que el contexto carga las relaciones
            // var tieneServicios = await _servicioRepo.AnyAsync(s => s.CategoriaId == id);
            // if (tieneServicios)
            //     throw new BusinessException("No se puede eliminar la categoría porque tiene servicios asociados");

            _repository.Delete(existente);
            await _repository.SaveAsync();

            _logger.LogInformation("Categoría eliminada exitosamente: {CategoriaId}", id);
        }



        /// <summary>
        /// Obtiene solo categorías activas
        /// </summary>
        public async Task<IEnumerable<CategoriaDto>> GetActiveAsync()
        {
            var categorias = await _repository.FindAsync(c => c.Estado == true);
            return categorias.Select(MapToDto);
        }

        /// <summary>
        /// Verifica si existe una categoría
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.AnyAsync(c => c.Id == id);
        }

        /// <summary>
        /// Activa o desactiva una categoría (soft delete)
        /// </summary>
        public async Task ToggleStatusAsync(int id, bool estado)
        {
            var categoria = await _repository.GetByIdAsync(id);
            if (categoria == null)
                throw new NotFoundException("Categoría", id);

            categoria.Estado = estado;
            _repository.Update(categoria);
            await _repository.SaveAsync();

            var accion = estado ? "activada" : "desactivada";
            _logger.LogInformation("Categoría {Accion}: {CategoriaId}", accion, id);
        }





        /// <summary>
        /// Mapea una entidad Categoria a CategoriaDto
        /// </summary>



        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        public async Task UpdateAsync(int id, CategoriaUpdateDto dto)
        {
            try
            {
                // Validar que el nombre no esté vacío
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new BusinessException("El nombre de la categoría es requerido");

                // Buscar la categoría existente
                var existente = await _repository.GetByIdAsync(id);
                if (existente == null)
                    throw new NotFoundException("Categoría", id);

                // Verificar si ya existe otra categoría con el mismo nombre (excluyendo la actual)
                var existeDuplicado = await _repository.AnyAsync(c =>
                    c.Nombre.ToLower() == dto.Nombre.ToLower().Trim() && c.Id != id);

                if (existeDuplicado)
                    throw new BusinessException(MensajesError.CategoriaYaExiste);

                // Actualizar datos
                existente.Nombre = dto.Nombre.Trim();
                existente.Descripcion = dto.Descripcion?.Trim();
                existente.Estado = dto.Estado;

                _repository.Update(existente);
                await _repository.SaveAsync();

                _logger.LogInformation("Categoría actualizada exitosamente: {CategoriaId} - {CategoriaNombre}",
                    id, dto.Nombre);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar categoría {CategoriaId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene categorías con paginación y búsqueda
        /// </summary>
        public async Task<PagedResult<CategoriaDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                // Validar parámetros de paginación
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Límite máximo

                // Construir el predicado de búsqueda
                System.Linq.Expressions.Expression<Func<Categoria, bool>>? predicate = null;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    predicate = c => c.Nombre.ToLower().Contains(term) ||
                                    (c.Descripcion != null && c.Descripcion.ToLower().Contains(term));
                }

                // Obtener datos paginados
                var (categorias, totalCount) = await _repository.GetPagedWithTotalAsync(
                    pageNumber,
                    pageSize,
                    predicate
                );

                // Mapear a DTO
                var items = categorias.Select(c => MapToDto(c));

                return new PagedResult<CategoriaDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías paginadas");
                throw;
            }
        }

        /// <summary>
        /// Verifica si existe una categoría con el nombre especificado
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return false;

                var nombreNormalizado = nombre.Trim().ToLower();

                if (excludeId.HasValue)
                {
                    // Verificar excluyendo un ID específico (para actualizaciones)
                    return await _repository.AnyAsync(c =>
                        c.Nombre.ToLower() == nombreNormalizado &&
                        c.Id != excludeId.Value);
                }

                // Verificar sin excluir (para crear nuevos)
                return await _repository.AnyAsync(c => c.Nombre.ToLower() == nombreNormalizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de categoría por nombre: {Nombre}", nombre);
                throw;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de servicios asociados a una categoría
        /// </summary>
        public async Task<int> GetServiciosCountAsync(int id)
        {
            try
            {
                // Verificar que la categoría existe
                var existe = await _repository.AnyAsync(c => c.Id == id);
                if (!existe)
                    throw new NotFoundException("Categoría", id);

                // Contar servicios asociados
                return await _servicioRepository.CountAsync(s => s.CategoriaId == id);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al contar servicios de categoría {CategoriaId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las categorías con el conteo de servicios asociados
        /// </summary>
        public async Task<IEnumerable<CategoriaConConteoDto>> GetCategoriasWithServiciosCountAsync()
        {
            try
            {
                // Obtener todas las categorías
                var categorias = await _repository.GetAllAsync();

                // Crear lista de resultados
                var resultados = new List<CategoriaConConteoDto>();

                foreach (var categoria in categorias)
                {
                    // Contar servicios activos y totales para esta categoría
                    var serviciosTotales = await _servicioRepository.CountAsync(s => s.CategoriaId == categoria.Id);
                    var serviciosActivos = await _servicioRepository.CountAsync(s =>
                        s.CategoriaId == categoria.Id && s.Estado == true);

                    resultados.Add(new CategoriaConConteoDto
                    {
                        Id = categoria.Id,
                        Nombre = categoria.Nombre,
                        Descripcion = categoria.Descripcion,
                        Estado = categoria.Estado ?? true,
                        CantidadServicios = serviciosTotales,
                        CantidadServiciosActivos = serviciosActivos
                    });
                }

                // Ordenar por cantidad de servicios (descendente)
                return resultados.OrderByDescending(r => r.CantidadServicios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías con conteo de servicios");
                throw;
            }
        }



        /// <summary>
        /// Obtiene solo las categorías que tienen servicios activos
        /// </summary>
        public async Task<IEnumerable<CategoriaDto>> GetCategoriasConServiciosAsync()
        {
            try
            {
                var categorias = await _repository.GetAllAsync();
                var resultado = new List<CategoriaDto>();

                foreach (var categoria in categorias)
                {
                    var tieneServicios = await _servicioRepository.AnyAsync(s =>
                        s.CategoriaId == categoria.Id && s.Estado == true);

                    if (tieneServicios)
                    {
                        resultado.Add(MapToDto(categoria));
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías con servicios");
                throw;
            }
        }

        /// <summary>
        /// Eliminación lógica (soft delete) - Desactiva la categoría
        /// </summary>
        public async Task SoftDeleteAsync(int id)
        {
            try
            {
                var existente = await _repository.GetByIdAsync(id);
                if (existente == null)
                    throw new NotFoundException("Categoría", id);

                // Verificar si tiene servicios activos
                var tieneServiciosActivos = await _servicioRepository.AnyAsync(s =>
                    s.CategoriaId == id && s.Estado == true);

                if (tieneServiciosActivos)
                    throw new BusinessException("No se puede desactivar la categoría porque tiene servicios activos asociados");

                existente.Estado = false;
                _repository.Update(existente);
                await _repository.SaveAsync();

                _logger.LogInformation("Categoría desactivada exitosamente: {CategoriaId}", id);
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al desactivar categoría {CategoriaId}", id);
                throw;
            }
        }



        private static CategoriaDto MapToDto(Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Estado = categoria.Estado ?? true
            };
        }


    }
}