using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class ResenaConfiguration:IEntityTypeConfiguration<Resena>
    {
        public void Configure(EntityTypeBuilder<Resena> builder)
        {
            builder.HasKey(e => e.Id).HasName("resenas_pkey");

            builder.ToTable("resenas");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Calificacion).HasColumnName("calificacion");
            builder.Property(e => e.Comentario).HasColumnName("comentario");
            builder.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            builder.Property(e => e.ServicioId).HasColumnName("servicio_id");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            builder.HasOne(d => d.Servicio).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.ServicioId)
                .HasConstraintName("resenas_servicio_id_fkey");

            builder.HasOne(d => d.Usuario).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("resenas_usuario_id_fkey");
        }
    }   
}