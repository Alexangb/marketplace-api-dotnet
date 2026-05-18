public class ServicioCreateDto
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
    public int CategoriaId { get; set; }
    public int UsuarioId { get; set; } // Por ahora lo pedimos manual ya que no hay login
}