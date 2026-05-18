namespace MarketplaceApi.Application.DTOs
{
    public class DashboardResumenDto
    {
        public int TotalCitasHoy { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasPendientes { get; set; } // 
        public int CitasCanceladas { get; set; } // 
        public decimal GananciasDelDia { get; set; }
        public decimal GananciasPendientes { get; set; }//Servicios no completados aún
        public double TasaOcupacion { get; set; } // Porcentaje de ocupación
        public IEnumerable<DashboardReservaDto> Reservas { get; set; } = new List<DashboardReservaDto>();
        // Propiedades calculadas
        public string GananciasFormateadas => GananciasDelDia.ToString("C");
        public int TotalCitas => TotalCitasHoy;
    }
}