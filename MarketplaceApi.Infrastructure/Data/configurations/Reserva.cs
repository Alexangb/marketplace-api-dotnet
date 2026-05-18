using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.HasKey(e => e.Id).HasName("reservas_pkey");

            builder.ToTable("reservas");

            builder.HasIndex(e => e.UsuarioId, "idx_reserva_usuario");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pendiente'::character varying")
                .HasColumnName("estado");
            builder.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            builder.Property(e => e.FechaReserva).HasColumnName("fecha_reserva");
            builder.Property(e => e.HoraFin).HasColumnName("hora_fin");
            builder.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            builder.Property(e => e.ServicioId).HasColumnName("servicio_id");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            builder.HasOne(d => d.Servicio).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ServicioId)
                .HasConstraintName("reservas_servicio_id_fkey");

            builder.HasOne(d => d.Usuario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("reservas_usuario_id_fkey");
        }
    }
}