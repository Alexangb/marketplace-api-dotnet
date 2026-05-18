namespace MarketplaceApi.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime TokenExpiration { get; set; }
        public string Email { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public string Rol { get; set; } = null!;
        public int UsuarioId { get; set; }
    }
}