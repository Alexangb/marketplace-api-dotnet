namespace MarketplaceApi.Application.DTOs
{
    public class GananciaDiariaDto
    {
        public DateOnly Fecha { get; set; }
        public decimal TotalGanado { get; set; }
        public int ServiciosCompletados { get; set; }
    }
}