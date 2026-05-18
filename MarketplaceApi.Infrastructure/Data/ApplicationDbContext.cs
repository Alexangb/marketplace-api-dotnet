using System;
using System.Collections.Generic;
using MarketplaceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceApi.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
   
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<HorariosDisponible> HorariosDisponibles { get; set; }

    public virtual DbSet<Resena> Resenas { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
        
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
