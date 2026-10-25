using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[Route("api/categorias")]
public class CategoriasController : ControladorBase
{
    public CategoriasController(CafeteriaContext db) : base(db)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var categorias = await Db.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new
            {
                c.Id,
                c.Nombre,
                cantidadProductos = c.Productos.Count
            })
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        Categoria categoria = await Db.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound(new { mensaje = "No existe la categoría." });
        }

        return Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CategoriaRequest request)
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

        if (nombre.Length < 2)
        {
            return BadRequest(new { mensaje = "El nombre debe tener al menos 2 letras." });
        }

        bool existe = await Db.Categorias
            .AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());

        if (existe)
        {
            return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });
        }

        Categoria categoria = new Categoria { Nombre = nombre };

        Db.Categorias.Add(categoria);
        await Db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, CategoriaRequest request)
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

        Categoria categoria = await Db.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound(new { mensaje = "No existe la categoría." });
        }

        string nombre = (request.Nombre ?? "").Trim();

        if (nombre.Length < 2)
        {
            return BadRequest(new { mensaje = "El nombre debe tener al menos 2 letras." });
        }

        bool existe = await Db.Categorias
            .AnyAsync(c => c.Id != id && c.Nombre.ToLower() == nombre.ToLower());

        if (existe)
        {
            return Conflict(new { mensaje = "Ya existe otra categoría con ese nombre." });
        }

        categoria.Nombre = nombre;
        await Db.SaveChangesAsync();

        return Ok(categoria);
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

        Categoria categoria = await Db.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound(new { mensaje = "No existe la categoría." });
        }

        bool tieneProductos = await Db.Productos.AnyAsync(p => p.CategoriaId == id);

        if (tieneProductos)
        {
            return Conflict(new
            {
                mensaje = "No puedes borrar la categoría porque todavía tiene productos."
            });
        }

        Db.Categorias.Remove(categoria);
        await Db.SaveChangesAsync();

        return Ok(new { mensaje = "Categoría eliminada." });
    }
}

public class CategoriaRequest
{
    public string Nombre { get; set; }
}
