using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[Route("api/admin")]
public class AdminController : ControladorBase
{
    public AdminController(CafeteriaContext db) : base(db)
    {
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen()
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        if (usuario.Rol != "admin")
        {
            return ErrorSoloAdmin();
        }

        DateTime hoy = Reloj.Hoy();
        DateTime manana = hoy.AddDays(1);

        List<Pedido> pedidosHoy = await Db.Pedidos
            .Where(p => p.CreadoEn >= hoy && p.CreadoEn < manana)
            .ToListAsync();

        List<int> idsPedidosValidos = pedidosHoy
            .Where(p => p.Estado != "Cancelado")
            .Select(p => p.Id)
            .ToList();

        List<PedidoItem> itemsHoy = await Db.PedidoItems
            .Include(i => i.Producto)
            .Where(i => idsPedidosValidos.Contains(i.PedidoId))
            .ToListAsync();

        var topProductos = itemsHoy
            .GroupBy(i => i.Producto.Nombre)
            .Select(g => new
            {
                nombre = g.Key,
                cantidad = g.Sum(i => i.Cantidad)
            })
            .OrderByDescending(x => x.cantidad)
            .Take(5)
            .ToList();

        int pendientes = await Db.Pedidos.CountAsync(p => p.Estado == "Pendiente");
        int enPreparacion = await Db.Pedidos.CountAsync(p => p.Estado == "EnPreparacion");
        int listos = await Db.Pedidos.CountAsync(p => p.Estado == "Listo");
        int clientes = await Db.Usuarios.CountAsync(u => u.Rol == "cliente");
        int productos = await Db.Productos.CountAsync();

        decimal ventasHoy = pedidosHoy
            .Where(p => p.Estado != "Cancelado")
            .Sum(p => p.Total);

        return Ok(new
        {
            pedidosHoy = pedidosHoy.Count,
            ventasHoy = Math.Round(ventasHoy, 2),
            pendientes,
            enPreparacion,
            listos,
            clientes,
            productos,
            topProductos
        });
    }
}
