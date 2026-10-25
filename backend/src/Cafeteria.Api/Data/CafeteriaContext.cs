using Cafeteria.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Data;

public class CafeteriaContext : DbContext
{
    public CafeteriaContext(DbContextOptions<CafeteriaContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Sesion>()
            .HasIndex(s => s.Token)
            .IsUnique();

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PedidoItem>()
            .HasOne(i => i.Pedido)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PedidoItem>()
            .HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sesion>()
            .HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
