using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;   

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class HorariosDisponibleConfiguration : IEntityTypeConfiguration<HorariosDisponible>
    {
        public void Configure(EntityTypeBuilder<HorariosDisponible> builder)
        {
             builder.HasKey(e => e.Id).HasName("horarios_disponibles_pkey");

            builder.ToTable("horarios_disponibles");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.DiaSemana).HasColumnName("dia_semana");
            builder.Property(e => e.HoraFin).HasColumnName("hora_fin");
            builder.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            builder.HasOne(d => d.Usuario).WithMany(p => p.HorariosDisponibles)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("horarios_disponibles_usuario_id_fkey");
        }
    }
}