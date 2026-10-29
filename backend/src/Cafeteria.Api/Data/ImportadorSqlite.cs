using Cafeteria.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Data;

public static class ImportadorSqlite
{
    public static void Importar(CafeteriaContext destino, string rutaSqlite)
    {
        if (!File.Exists(rutaSqlite) || destino.Usuarios.Any())
        {
            return;
        }

        DbContextOptions<CafeteriaContext> opciones = new DbContextOptionsBuilder<CafeteriaContext>()
            .UseSqlite("Data Source=" + rutaSqlite)
            .Options;

        using CafeteriaContext origen = new CafeteriaContext(opciones);

        List<Categoria> categorias = origen.Categorias.AsNoTracking().ToList();
        List<Producto> productos = origen.Productos.AsNoTracking().ToList();
        List<Usuario> usuarios = origen.Usuarios.AsNoTracking().ToList();
        List<Sesion> sesiones = origen.Sesiones.AsNoTracking().ToList();
        List<Pedido> pedidos = origen.Pedidos
            .AsNoTracking()
            .Include(p => p.Items)
            .ToList();

        using var transaccion = destino.Database.BeginTransaction();

        Dictionary<int, Categoria> mapaCategorias = new Dictionary<int, Categoria>();

        foreach (Categoria categoria in categorias)
        {
            Categoria nueva = new Categoria { Nombre = categoria.Nombre };
            destino.Categorias.Add(nueva);
            mapaCategorias[categoria.Id] = nueva;
        }

        destino.SaveChanges();

        Dictionary<int, Producto> mapaProductos = new Dictionary<int, Producto>();

        foreach (Producto producto in productos)
        {
            if (!mapaCategorias.TryGetValue(producto.CategoriaId, out Categoria categoria))
            {
                continue;
            }

            Producto nuevo = new Producto
            {
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                ImgUrl = producto.ImgUrl,
                Disponible = producto.Disponible,
                Categoria = categoria
            };

            destino.Productos.Add(nuevo);
            mapaProductos[producto.Id] = nuevo;
        }

        destino.SaveChanges();

        Dictionary<int, Usuario> mapaUsuarios = new Dictionary<int, Usuario>();

        foreach (Usuario usuario in usuarios)
        {
            Usuario nuevo = new Usuario
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                PasswordHash = usuario.PasswordHash,
                Rol = usuario.Rol
            };

            destino.Usuarios.Add(nuevo);
            mapaUsuarios[usuario.Id] = nuevo;
        }

        destino.SaveChanges();

        foreach (Sesion sesion in sesiones)
        {
            if (!mapaUsuarios.TryGetValue(sesion.UsuarioId, out Usuario usuario))
            {
                continue;
            }

            destino.Sesiones.Add(new Sesion
            {
                Token = sesion.Token,
                ExpiraEn = sesion.ExpiraEn,
                Usuario = usuario
            });
        }

        destino.SaveChanges();

        foreach (Pedido pedido in pedidos)
        {
            if (!mapaUsuarios.TryGetValue(pedido.UsuarioId, out Usuario usuario))
            {
                continue;
            }

            Pedido nuevo = new Pedido
            {
                Usuario = usuario,
                Tipo = pedido.Tipo,
                NumeroMesa = pedido.NumeroMesa,
                MetodoPago = pedido.MetodoPago,
                Estado = pedido.Estado,
                Total = pedido.Total,
                Notas = pedido.Notas,
                CreadoEn = pedido.CreadoEn
            };

            foreach (PedidoItem item in pedido.Items)
            {
                if (!mapaProductos.TryGetValue(item.ProductoId, out Producto producto))
                {
                    continue;
                }

                nuevo.Items.Add(new PedidoItem
                {
                    Producto = producto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Notas = item.Notas
                });
            }

            destino.Pedidos.Add(nuevo);
        }

        destino.SaveChanges();

        transaccion.Commit();
    }
}
