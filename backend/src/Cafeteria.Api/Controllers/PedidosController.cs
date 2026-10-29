using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[Route("api/pedidos")]
public class PedidosController : ControladorBase
{
    private static readonly string[] EstadosValidos =
    {
        "Pendiente",
        "EnPreparacion",
        "Listo",
        "Entregado",
        "Cancelado"
    };

    private static readonly Dictionary<string, string[]> CambiosPermitidos =
        new Dictionary<string, string[]>
        {
            { "Pendiente", new[] { "EnPreparacion", "Cancelado" } },
            { "EnPreparacion", new[] { "Listo", "Cancelado" } },
            { "Listo", new[] { "Entregado", "Cancelado" } },
            { "Entregado", new string[0] },
            { "Cancelado", new string[0] }
        };

    public PedidosController(CafeteriaContext db) : base(db)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Listar(string estado)
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        if (!string.IsNullOrWhiteSpace(estado) && !EstadosValidos.Contains(estado))
        {
            return BadRequest(new { mensaje = "Ese estado no existe." });
        }

        IQueryable<Pedido> consulta = Db.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto);

        if (usuario.Rol != "admin")
        {
            consulta = consulta.Where(p => p.UsuarioId == usuario.Id);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            consulta = consulta.Where(p => p.Estado == estado);
        }

        List<Pedido> pedidos = await consulta
            .OrderByDescending(p => p.CreadoEn)
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        Pedido pedido = await Db.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound(new { mensaje = "No existe el pedido." });
        }

        if (usuario.Rol != "admin" && pedido.UsuarioId != usuario.Id)
        {
            return StatusCode(403, new { mensaje = "No puedes ver el pedido de otra persona." });
        }

        return Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(PedidoRequest request)
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        if (request == null)
        {
            return BadRequest(new { mensaje = "Faltan los datos del pedido." });
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest(new { mensaje = "El pedido debe tener al menos un producto." });
        }

        string tipo = request.Tipo ?? "";

        if (tipo != "ParaLlevar" && tipo != "Mesa")
        {
            return BadRequest(new { mensaje = "El tipo debe ser ParaLlevar o Mesa." });
        }

        if (tipo == "Mesa" && (!request.NumeroMesa.HasValue || request.NumeroMesa.Value < 1))
        {
            return BadRequest(new { mensaje = "Escribe el número de mesa." });
        }

        string metodoPago = request.MetodoPago ?? "";

        if (metodoPago != "Efectivo" && metodoPago != "Tarjeta")
        {
            return BadRequest(new { mensaje = "El método de pago debe ser Efectivo o Tarjeta." });
        }

        string notas = (request.Notas ?? "").Trim();

        if (notas.Length > 300)
        {
            return BadRequest(new { mensaje = "Las observaciones son demasiado largas." });
        }

        Pedido pedido = new Pedido
        {
            UsuarioId = usuario.Id,
            Usuario = usuario,
            Tipo = tipo,
            NumeroMesa = tipo == "Mesa" ? request.NumeroMesa : null,
            MetodoPago = metodoPago,
            Estado = "Pendiente",
            Notas = notas,
            CreadoEn = Reloj.Ahora(),
            Total = 0
        };

        foreach (PedidoItemRequest item in request.Items)
        {
            if (item.Cantidad < 1 || item.Cantidad > 20)
            {
                return BadRequest(new { mensaje = "La cantidad debe estar entre 1 y 20." });
            }

            Producto producto = await Db.Productos
                .FirstOrDefaultAsync(p => p.Id == item.ProductoId);

            if (producto == null)
            {
                return BadRequest(new { mensaje = "Uno de los productos no existe." });
            }

            if (!producto.Disponible)
            {
                return BadRequest(new
                {
                    mensaje = "El producto '" + producto.Nombre + "' no está disponible."
                });
            }

            string notasItem = (item.Notas ?? "").Trim();

            if (notasItem.Length > 150)
            {
                return BadRequest(new { mensaje = "Una nota de producto es demasiado larga." });
            }

            PedidoItem linea = new PedidoItem
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio,
                Notas = notasItem
            };

            pedido.Items.Add(linea);
            pedido.Total = pedido.Total + (producto.Precio * item.Cantidad);
        }

        pedido.Total = Math.Round(pedido.Total, 2);

        Db.Pedidos.Add(pedido);
        await Db.SaveChangesAsync();

        return StatusCode(201, pedido);
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, EstadoRequest request)
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Estado))
        {
            return BadRequest(new { mensaje = "Escribe el nuevo estado." });
        }

        string nuevoEstado = request.Estado.Trim();

        if (!EstadosValidos.Contains(nuevoEstado))
        {
            return BadRequest(new { mensaje = "Ese estado no existe." });
        }

        Pedido pedido = await Db.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Items)
                .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound(new { mensaje = "No existe el pedido." });
        }

        if (usuario.Rol != "admin")
        {
            if (pedido.UsuarioId != usuario.Id)
            {
                return StatusCode(403, new { mensaje = "No puedes cambiar el pedido de otra persona." });
            }

            if (pedido.Estado != "Pendiente" || nuevoEstado != "Cancelado")
            {
                return StatusCode(403, new
                {
                    mensaje = "Solo puedes cancelar tus pedidos que todavía están pendientes."
                });
            }
        }

        if (pedido.Estado == nuevoEstado)
        {
            return Ok(pedido);
        }

        if (!CambiosPermitidos.TryGetValue(pedido.Estado, out string[] permitidos))
        {
            return BadRequest(new { mensaje = "El pedido tiene un estado desconocido." });
        }

        if (!permitidos.Contains(nuevoEstado))
        {
            return BadRequest(new
            {
                mensaje = "No se puede pasar de " + pedido.Estado + " a " + nuevoEstado + "."
            });
        }

        pedido.Estado = nuevoEstado;
        await Db.SaveChangesAsync();

        return Ok(pedido);
    }
}

public class PedidoRequest
{
    public string Tipo { get; set; } = "ParaLlevar";
    public int? NumeroMesa { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string Notas { get; set; } = "";
    public List<PedidoItemRequest> Items { get; set; } = new List<PedidoItemRequest>();
}

public class PedidoItemRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string Notas { get; set; } = "";
}

public class EstadoRequest
{
    public string Estado { get; set; }
}
