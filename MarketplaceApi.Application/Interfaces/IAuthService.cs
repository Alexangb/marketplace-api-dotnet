using MarketplaceApi.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace MarketplaceApi.Application.Interfaces
{
    /// <summary>
    /// Servicio para autenticación y gestión de usuarios
    /// </summary>
    public interface IAuthService
    {
        // Registro y autenticación
        Task<bool> RegisterAsync(UserRegisterDto userDto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> LogoutAsync(int usuarioId);
        
        // Perfil de usuario
        Task<UsuarioPerfilDto> ObtenerPerfilAsync(int usuarioId);
        Task<UsuarioPerfilDto> UpdatePerfilAsync(int usuarioId, UsuarioUpdateDto dto);
        Task<bool> ChangePasswordAsync(int usuarioId, ChangePasswordDto dto);
        Task<string> SubirFotoPerfilAsync(int usuarioId, IFormFile archivo);
        Task<bool> EliminarFotoPerfilAsync(int usuarioId);
        
        // Validaciones
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByIdAsync(int usuarioId);
        Task<bool> ValidatePasswordAsync(int usuarioId, string password);
        Task<UsuarioDto?> GetUsuarioByIdAsync(int usuarioId);
        Task<UsuarioDto?> GetUsuarioByEmailAsync(string email);
        
        // Gestión de usuarios (Admin)
        Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
        Task<IEnumerable<UsuarioDto>> GetUsuariosByRolAsync(string rol);
        Task<bool> UpdateRolAsync(int usuarioId, string nuevoRol);
        Task<bool> ToggleUserStatusAsync(int usuarioId, bool estado);
        Task<bool> DeleteUsuarioAsync(int usuarioId);
        
        // Recuperación de contraseña
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> VerifyEmailAsync(string email, string token);
        
        // Tokens y sesión
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(int usuarioId);
        
        // Estadísticas
        Task<int> GetTotalUsuariosAsync();
        Task<Dictionary<string, int>> GetUsuariosPorRolAsync();
        Task<int> GetUsuariosActivosAsync();
    }
}