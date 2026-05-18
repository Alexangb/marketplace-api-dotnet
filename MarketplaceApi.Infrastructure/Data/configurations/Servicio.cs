using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.HasKey(e => e.Id).HasName("servicios_pkey");

            builder.ToTable("servicios");

            builder.HasIndex(e => e.CategoriaId, "idx_servicio_categoria");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            builder.Property(e => e.Descripcion).HasColumnName("descripcion");
            builder.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");
            builder.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            builder.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            builder.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            builder.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            builder.HasOne(d => d.Categoria).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("servicios_categoria_id_fkey");

            builder.HasOne(d => d.Usuario).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("servicios_usuario_id_fkey");
        }
    }
}
