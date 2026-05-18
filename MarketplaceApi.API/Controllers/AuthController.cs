using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Shared.utilities;
using MarketplaceApi.Shared.exceptions;

namespace MarketplaceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Registro y Autenticación

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="userDto">Datos del usuario a registrar</param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(userDto);
                return Ok(ApiResponse.Ok("Usuario registrado exitosamente"));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Inicia sesión en el sistema
        /// </summary>
        /// <param name="dto">Credenciales de acceso</param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(ApiResponse<LoginResponseDto>.Ok(response!, "Inicio de sesión exitoso"));
            }
            catch (BusinessException ex)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = GetCurrentUserId();
            await _authService.LogoutAsync(usuarioId);
            return Ok(ApiResponse.Ok("Sesión cerrada exitosamente"));
        }

        /// <summary>
        /// Refresca el token de autenticación
        /// </summary>
        /// <param name="refreshToken">Token de refresco</param>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshToken);
                if (response == null)
                    return Unauthorized(ApiResponse<LoginResponseDto>.Error("Token de refresco inválido"));

                return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Token refrescado exitosamente"));
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Revoca el token del usuario actual
        /// </summary>
        [Authorize]
        [HttpPost("revoke-token")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeToken()
        {
            var usuarioId = GetCurrentUserId();
            await _authService.RevokeTokenAsync(usuarioId);
            return Ok(ApiResponse.Ok("Token revocado exitosamente"));
        }

        #endregion

        #region Perfil de Usuario

        /// <summary>
        /// Obtiene el perfil del usuario autenticado
        /// </summary>
        [Authorize]
        [HttpGet("perfil")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioPerfilDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMiPerfil()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var perfil = await _authService.ObtenerPerfilAsync(usuarioId);
                return Ok(ApiResponse<UsuarioPerfilDto>.Ok(perfil));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<UsuarioPerfilDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene el perfil de un usuario por su ID (público)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpGet("perfil/{id}")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioPerfilDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPerfilById(int id)
        {
            try
            {
                var perfil = await _authService.ObtenerPerfilAsync(id);
                return Ok(ApiResponse<UsuarioPerfilDto>.Ok(perfil));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<UsuarioPerfilDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Actualiza el perfil del usuario autenticado
        /// </summary>
        /// <param name="dto">Datos actualizados</param>
        [Authorize]
        [HttpPut("perfil")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioPerfilDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePerfil([FromBody] UsuarioUpdateDto dto)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var perfil = await _authService.UpdatePerfilAsync(usuarioId, dto);
                return Ok(ApiResponse<UsuarioPerfilDto>.Ok(perfil, "Perfil actualizado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<UsuarioPerfilDto>.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<UsuarioPerfilDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Sube o actualiza la foto de perfil
        /// </summary>
        /// <param name="dto">Archivo de imagen</param>
        [Authorize]
        [HttpPost("perfil/foto")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubirFoto([FromForm] SubirImagenDto dto)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                var url = await _authService.SubirFotoPerfilAsync(usuarioId, dto.Archivo);
                return Ok(ApiResponse<string>.Ok(url, "Foto de perfil actualizada exitosamente"));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<string>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Elimina la foto de perfil
        /// </summary>
        [Authorize]
        [HttpDelete("perfil/foto")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarFoto()
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                await _authService.EliminarFotoPerfilAsync(usuarioId);
                return Ok(ApiResponse.Ok("Foto de perfil eliminada exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario autenticado
        /// </summary>
        /// <param name="dto">Datos para cambio de contraseña</param>
        [Authorize]
        [HttpPost("perfil/cambiar-password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var usuarioId = GetCurrentUserId();
                await _authService.ChangePasswordAsync(usuarioId, dto);
                return Ok(ApiResponse.Ok("Contraseña cambiada exitosamente"));
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

        #endregion

        #region Recuperación de Contraseña

        /// <summary>
        /// Solicita recuperación de contraseña
        /// </summary>
        /// <param name="email">Email del usuario</param>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            await _authService.ForgotPasswordAsync(email);
            // Siempre retornamos OK por seguridad (no revelamos si el email existe)
            return Ok(ApiResponse.Ok("Si el email existe, recibirás instrucciones para recuperar tu contraseña"));
        }

        /// <summary>
        /// Restablece la contraseña usando un token
        /// </summary>
        /// <param name="dto">Datos para restablecer contraseña</param>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto);
                return Ok(ApiResponse.Ok("Contraseña restablecida exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ApiResponse.Error(ex.Message));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse.Error(ex.Message));
            }
        }

        #endregion

        #region Administración de Usuarios (Requiere Admin)

        /// <summary>
        /// Obtiene todos los usuarios (Solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/usuarios")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsuarioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _authService.GetAllUsuariosAsync();
            return Ok(ApiResponse<IEnumerable<UsuarioDto>>.Ok(usuarios));
        }

        /// <summary>
        /// Obtiene usuarios por rol (Solo Admin)
        /// </summary>
        /// <param name="rol">Rol a filtrar</param>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/usuarios/rol/{rol}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsuarioDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsuariosByRol(string rol)
        {
            var usuarios = await _authService.GetUsuariosByRolAsync(rol);
            return Ok(ApiResponse<IEnumerable<UsuarioDto>>.Ok(usuarios));
        }

        /// <summary>
        /// Actualiza el rol de un usuario (Solo Admin)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="nuevoRol">Nuevo rol</param>
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/usuarios/{id}/rol")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] string nuevoRol)
        {
            try
            {
                await _authService.UpdateRolAsync(id, nuevoRol);
                return Ok(ApiResponse.Ok($"Rol del usuario {id} actualizado a {nuevoRol}"));
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
        /// Activa o desactiva un usuario (Solo Admin)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="estado">true = activar, false = desactivar</param>
        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/usuarios/{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleUserStatus(int id, [FromQuery] bool estado)
        {
            try
            {
                await _authService.ToggleUserStatusAsync(id, estado);
                var mensaje = estado ? "Usuario activado" : "Usuario desactivado";
                return Ok(ApiResponse.Ok($"{mensaje} exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        /// <summary>
        /// Elimina un usuario (Solo Admin)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/usuarios/{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                await _authService.DeleteUsuarioAsync(id);
                return Ok(ApiResponse.Ok($"Usuario {id} eliminado exitosamente"));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse.Error(ex.Message));
            }
        }

        #endregion

        #region Validaciones

        /// <summary>
        /// Verifica si un email ya está registrado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        [HttpGet("verify-email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var exists = await _authService.ExistsByEmailAsync(email);
            return Ok(ApiResponse<bool>.Ok(!exists, exists ? "Email ya registrado" : "Email disponible"));
        }

        /// <summary>
        /// Verifica si un usuario existe
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpGet("verify-user/{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyUser(int id)
        {
            var exists = await _authService.ExistsByIdAsync(id);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        #endregion

        #region Estadísticas (Requiere Admin)

        /// <summary>
        /// Obtiene el total de usuarios (Solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/estadisticas/total")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalUsuarios()
        {
            var total = await _authService.GetTotalUsuariosAsync();
            return Ok(ApiResponse<int>.Ok(total));
        }

        /// <summary>
        /// Obtiene usuarios agrupados por rol (Solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/estadisticas/por-rol")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsuariosPorRol()
        {
            var estadisticas = await _authService.GetUsuariosPorRolAsync();
            return Ok(ApiResponse<Dictionary<string, int>>.Ok(estadisticas));
        }

        /// <summary>
        /// Obtiene el total de usuarios activos (Solo Admin)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/estadisticas/activos")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsuariosActivos()
        {
            var total = await _authService.GetUsuariosActivosAsync();
            return Ok(ApiResponse<int>.Ok(total));
        }

        #endregion

        #region Métodos Privados

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario");

            return int.Parse(userIdClaim);
        }

        #endregion
    }
}