using MarketplaceApi.Application.DTOs;
using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Domain.Entities;
using MarketplaceApi.Domain.Interfaces;
using MarketplaceApi.Shared.constants;
using MarketplaceApi.Shared.exceptions;
using MarketplaceApi.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MarketplaceApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly string _fotosPath;
        // Agregar el servicio en el constructor
        private readonly CloudinaryService _cloudinaryService;


        public AuthService(
            IGenericRepository<Usuario> usuarioRepo,
            ILogger<AuthService> logger, JwtService jwtService, CloudinaryService cloudinaryService
            )
        {
            _usuarioRepo = usuarioRepo;
            _logger = logger;
            _jwtService = jwtService;
            _cloudinaryService = cloudinaryService;
            _fotosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "perfiles");
        }

        public async Task<bool> RegisterAsync(UserRegisterDto userDto)
        {
            try
            {
                // Validar que el email no exista
                var emailExiste = await _usuarioRepo.AnyAsync(u => u.Email == userDto.Email);
                if (emailExiste)
                    throw new BusinessException("El email ya está registrado");

                // Validar rol válido
                if (!Roles.IsValidRole(userDto.Rol))
                    throw new BusinessException($"Rol '{userDto.Rol}' no es válido");

                // Hashear contraseña
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                var usuario = new Usuario
                {
                    Nombre = userDto.Nombre.Trim(),
                    Apellido = userDto.Apellido?.Trim(),
                    Email = userDto.Email.ToLower().Trim(),
                    PasswordHash = passwordHash,
                    Rol = userDto.Rol,
                    Estado = true,
                    FechaRegistro = DateTime.UtcNow
                };

                await _usuarioRepo.AddAsync(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Usuario registrado exitosamente: {Email}", usuario.Email);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException)
            {
                _logger.LogError(ex, "Error al registrar usuario {Email}", userDto.Email);
                throw;
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            try
            {
                var usuario = await _usuarioRepo.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (usuario == null)
                    throw new BusinessException("Credenciales inválidas");

                if (usuario.Estado == false)
                    throw new BusinessException("Usuario inactivo. Contacte al administrador");

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                    throw new BusinessException("Credenciales inválidas");

                //  Generar token real
                var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.Rol);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Guardar refresh token en BD
                usuario.RefreshToken = refreshToken;
                usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                usuario.UltimoAcceso = DateTime.UtcNow;

                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Usuario logueado exitosamente: {Email}", usuario.Email);

                return new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiration = DateTime.UtcNow.AddHours(8),
                    Email = usuario.Email,
                    Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                    FotoUrl = usuario.FotoUrl,
                    Rol = usuario.Rol,
                    UsuarioId = usuario.Id
                };
            }
            catch (Exception ex) when (ex is not BusinessException)
            {
                _logger.LogError(ex, "Error en login para {Email}", dto.Email);
                throw;
            }
        }

        public async Task<bool> LogoutAsync(int usuarioId)
        {
            try
            {
                // Implementar lógica de logout (revocar token, etc.)
                _logger.LogInformation("Usuario {UsuarioId} cerró sesión", usuarioId);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en logout para usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<UsuarioPerfilDto> ObtenerPerfilAsync(int usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                return new UsuarioPerfilDto
                {
                    Id = usuario.Id,
                    Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    FotoUrl = usuario.FotoUrl,
                    Rol = usuario.Rol,
                    FechaRegistro = usuario.FechaRegistro
                };
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al obtener perfil de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<UsuarioPerfilDto> UpdatePerfilAsync(int usuarioId, UsuarioUpdateDto dto)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                // Verificar email único (excluyendo el actual)
                if (usuario.Email != dto.Email)
                {
                    var emailExiste = await _usuarioRepo.AnyAsync(u => u.Email == dto.Email && u.Id != usuarioId);
                    if (emailExiste)
                        throw new BusinessException("El email ya está registrado por otro usuario");
                }

                usuario.Nombre = dto.Nombre.Trim();
                usuario.Apellido = dto.Apellido?.Trim();
                usuario.Email = dto.Email.ToLower().Trim();

                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Perfil actualizado para usuario {UsuarioId}", usuarioId);

                return new UsuarioPerfilDto
                {
                    Id = usuario.Id,
                    Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                    Email = usuario.Email,
                    FotoUrl = usuario.FotoUrl,
                    Rol = usuario.Rol,
                    FechaRegistro = usuario.FechaRegistro
                };
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar perfil de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int usuarioId, ChangePasswordDto dto)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                // Verificar contraseña actual
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, usuario.PasswordHash))
                    throw new BusinessException("Contraseña actual incorrecta");

                // Hashear nueva contraseña
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Contraseña cambiada para usuario {UsuarioId}", usuarioId);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al cambiar contraseña de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }
        // Modificar SubirFotoPerfilAsync
        public async Task<string> SubirFotoPerfilAsync(int usuarioId, IFormFile archivo)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                // Validar archivo
                if (archivo == null || archivo.Length == 0)
                    throw new BusinessException("No se ha seleccionado ningún archivo");

                var extension = Path.GetExtension(archivo.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                    throw new BusinessException($"Formato no permitido. Use: {string.Join(", ", allowedExtensions)}");

                if (archivo.Length > 5 * 1024 * 1024) // 5MB
                    throw new BusinessException("El archivo no puede superar los 5MB");

                // Eliminar foto anterior si existe en Cloudinary
                if (!string.IsNullOrEmpty(usuario.FotoUrl))
                {
                    var publicId = _cloudinaryService.GetPublicIdFromUrl(usuario.FotoUrl);
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }

                // Subir nueva foto a Cloudinary
                var imageUrl = await _cloudinaryService.UploadImageAsync(archivo, "perfiles");

                // Actualizar URL en BD
                usuario.FotoUrl = imageUrl;
                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Foto de perfil actualizada para usuario {UsuarioId}", usuarioId);

                return usuario.FotoUrl;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al subir foto de perfil para usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        // Modificar EliminarFotoPerfilAsync
        public async Task<bool> EliminarFotoPerfilAsync(int usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                if (!string.IsNullOrEmpty(usuario.FotoUrl))
                {
                    // Eliminar de Cloudinary
                    var publicId = _cloudinaryService.GetPublicIdFromUrl(usuario.FotoUrl);
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }

                usuario.FotoUrl = null;
                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Foto de perfil eliminada para usuario {UsuarioId}", usuarioId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar foto de perfil de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _usuarioRepo.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByIdAsync(int usuarioId)
        {
            return await _usuarioRepo.AnyAsync(u => u.Id == usuarioId);
        }

        public async Task<bool> ValidatePasswordAsync(int usuarioId, string password)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
            if (usuario == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        }

        public async Task<UsuarioDto?> GetUsuarioByIdAsync(int usuarioId)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
            return usuario != null ? MapToDto(usuario) : null;
        }

        public async Task<UsuarioDto?> GetUsuarioByEmailAsync(string email)
        {
            var usuario = await _usuarioRepo.FirstOrDefaultAsync(u => u.Email == email);
            return usuario != null ? MapToDto(usuario) : null;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
        {
            var usuarios = await _usuarioRepo.GetAllAsync();
            return usuarios.Select(MapToDto);
        }

        public async Task<IEnumerable<UsuarioDto>> GetUsuariosByRolAsync(string rol)
        {
            var usuarios = await _usuarioRepo.FindAsync(u => u.Rol == rol);
            return usuarios.Select(MapToDto);
        }

        public async Task<bool> UpdateRolAsync(int usuarioId, string nuevoRol)
        {
            try
            {
                if (!Roles.IsValidRole(nuevoRol))
                    throw new BusinessException($"Rol '{nuevoRol}' no es válido");

                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                usuario.Rol = nuevoRol;
                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Rol actualizado para usuario {UsuarioId} a {NuevoRol}", usuarioId, nuevoRol);
                return true;
            }
            catch (Exception ex) when (ex is not BusinessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al actualizar rol de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int usuarioId, bool estado)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                usuario.Estado = estado;
                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Usuario {UsuarioId} {Accion}", usuarioId, estado ? "activado" : "desactivado");
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al cambiar estado de usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(usuarioId);
                if (usuario == null)
                    throw new NotFoundException("Usuario", usuarioId);

                // Eliminar foto si existe
                if (!string.IsNullOrEmpty(usuario.FotoUrl))
                {
                    var filePath = Path.Combine(_fotosPath, Path.GetFileName(usuario.FotoUrl));
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                _usuarioRepo.Delete(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Usuario eliminado exitosamente: {UsuarioId}", usuarioId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var usuario = await _usuarioRepo.FirstOrDefaultAsync(u => u.Email == email);
                if (usuario == null)
                    return false; // No revelamos si el email existe o no por seguridad

                // Generar token de recuperación
                var token = Guid.NewGuid().ToString();
                // Aquí implementar envío de email con el token

                _logger.LogInformation("Solicitud de recuperación de contraseña para {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en forgot password para {Email}", email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                var usuario = await _usuarioRepo.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (usuario == null)
                    throw new NotFoundException("Usuario", dto.Email);

                // Validar token (implementar lógica de validación de token)
                // if (!ValidateToken(dto.Token, usuario.Id)) throw new BusinessException("Token inválido");

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _usuarioRepo.Update(usuario);
                await _usuarioRepo.SaveAsync();

                _logger.LogInformation("Contraseña restablecida para usuario {Email}", dto.Email);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error al resetear contraseña para {Email}", dto.Email);
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            try
            {
                // Implementar verificación de email
                _logger.LogInformation("Email verificado para {Email}", email);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar email {Email}", email);
                return false;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Implementar lógica de refresh token
                _logger.LogInformation("Refresh token solicitado");
                return await Task.FromResult<LoginResponseDto?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al refrescar token");
                throw;
            }
        }

        public async Task<bool> RevokeTokenAsync(int usuarioId)
        {
            try
            {
                // Implementar lógica para revocar token
                _logger.LogInformation("Token revocado para usuario {UsuarioId}", usuarioId);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar token para usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<int> GetTotalUsuariosAsync()
        {
            return await _usuarioRepo.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetUsuariosPorRolAsync()
        {
            var usuarios = await _usuarioRepo.GetAllAsync();

            return usuarios
                .GroupBy(u => u.Rol)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        public async Task<int> GetUsuariosActivosAsync()
        {
            return await _usuarioRepo.CountAsync(u => u.Estado == true);
        }

        #region Métodos Privados

        private string GenerateJwtToken(Usuario usuario)
        {
            // Implementar generación de JWT token
            // Requiere: Microsoft.AspNetCore.Authentication.JwtBearer
            return "token_generado_aqui";
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private static UsuarioDto MapToDto(Usuario u)
        {
            return new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rol = u.Rol,
                FechaRegistro = u.FechaRegistro,
                Estado = u.Estado,
                FotoUrl = u.FotoUrl
            };
        }

        #endregion
    }
}