using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
           builder.HasKey(e => e.Id).HasName("usuarios_pkey");

            builder.ToTable("usuarios");

            builder.HasIndex(e => e.Email, "idx_usuario_email");

            builder.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            builder.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            builder.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            builder.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            builder.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            builder.Property(e => e.PasswordHash).HasColumnName("password_hash");
            builder.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasColumnName("rol");
        }
    }
}