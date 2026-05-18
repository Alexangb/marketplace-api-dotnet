namespace MarketplaceApi.Application.DTOs
{
    public class DashboardReservaDto
    {
        public int ReservaId { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string ServicioNombre { get; set; } = null!;
        public string Hora { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string? Notas { get; set; }
    }
}