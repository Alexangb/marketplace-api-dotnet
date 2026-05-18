using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketplaceApi.Domain.Entities;

namespace MarketplaceApi.Infrastructure.Data.Configurations
{
    public class CategoriaConfiguration:IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
             builder.HasKey(e => e.Id).HasName("categorias_pkey");

            builder.ToTable("categorias");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Descripcion).HasColumnName("descripcion");
            builder.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            builder.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        }
    }
}