using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[Route("api/productos")]
public class ProductosController : ControladorBase
{
    public ProductosController(CafeteriaContext db) : base(db)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        int? categoriaId,
        string buscar,
        bool soloDisponibles = false)
    {
        IQueryable<Producto> consulta = Db.Productos.Include(p => p.Categoria);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            string texto = buscar.Trim().ToLower();

            consulta = consulta.Where(p =>
                p.Nombre.ToLower().Contains(texto) ||
                p.Descripcion.ToLower().Contains(texto));
        }

        if (soloDisponibles)
        {
            consulta = consulta.Where(p => p.Disponible);
        }

        List<Producto> productos = await consulta
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return Ok(productos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        Producto producto = await Db.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "No existe el producto." });
        }

        return Ok(producto);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(ProductoRequest request)
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

        string nombre = (request.Nombre ?? "").Trim();
        string descripcion = (request.Descripcion ?? "").Trim();

        if (nombre.Length < 2)
        {
            return BadRequest(new { mensaje = "El nombre debe tener al menos 2 letras." });
        }

        if (request.Precio < 0)
        {
            return BadRequest(new { mensaje = "El precio no puede ser negativo." });
        }

        Categoria categoria = await Db.Categorias.FindAsync(request.CategoriaId);

        if (categoria == null)
        {
            return BadRequest(new { mensaje = "La categoría no existe." });
        }

        Producto producto = new Producto
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Precio = request.Precio,
            Disponible = request.Disponible,
            CategoriaId = categoria.Id,
            Categoria = categoria
        };

        Db.Productos.Add(producto);
        await Db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = producto.Id }, producto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, ProductoRequest request)
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

        Producto producto = await Db.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "No existe el producto." });
        }

        string nombre = (request.Nombre ?? "").Trim();
        string descripcion = (request.Descripcion ?? "").Trim();

        if (nombre.Length < 2)
        {
            return BadRequest(new { mensaje = "El nombre debe tener al menos 2 letras." });
        }

        if (request.Precio < 0)
        {
            return BadRequest(new { mensaje = "El precio no puede ser negativo." });
        }

        Categoria categoria = await Db.Categorias.FindAsync(request.CategoriaId);

        if (categoria == null)
        {
            return BadRequest(new { mensaje = "La categoría no existe." });
        }

        producto.Nombre = nombre;
        producto.Descripcion = descripcion;
        producto.Precio = request.Precio;
        producto.Disponible = request.Disponible;
        producto.CategoriaId = categoria.Id;
        producto.Categoria = categoria;

        await Db.SaveChangesAsync();

        return Ok(producto);
    }

    [HttpPatch("{id}/disponibilidad")]
    public async Task<IActionResult> CambiarDisponibilidad(int id, DisponibilidadRequest request)
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

        Producto producto = await Db.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "No existe el producto." });
        }

        producto.Disponible = request.Disponible;
        await Db.SaveChangesAsync();

        return Ok(producto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
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

        Producto producto = await Db.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound(new { mensaje = "No existe el producto." });
        }

        bool tienePedidos = await Db.PedidoItems.AnyAsync(i => i.ProductoId == id);

        if (tienePedidos)
        {
            return Conflict(new
            {
                mensaje = "No puedes borrar el producto porque aparece en pedidos. " +
                          "Márcalo como no disponible."
            });
        }

        Db.Productos.Remove(producto);
        await Db.SaveChangesAsync();

        return Ok(new { mensaje = "Producto eliminado." });
    }
}

public class ProductoRequest
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Disponible { get; set; } = true;
    public int CategoriaId { get; set; }
}

public class DisponibilidadRequest
{
    public bool Disponible { get; set; }
}
