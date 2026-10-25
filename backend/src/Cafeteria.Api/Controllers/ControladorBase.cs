using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[ApiController]
public abstract class ControladorBase : ControllerBase
{
    protected readonly CafeteriaContext Db;

    protected ControladorBase(CafeteriaContext db)
    {
        Db = db;
    }

    protected Usuario UsuarioActual()
    {
        string autorizacion = Request.Headers.Authorization.ToString();

        if (!autorizacion.StartsWith("Bearer "))
        {
            return null;
        }

        string token = autorizacion.Substring("Bearer ".Length).Trim();

        Sesion sesion = Db.Sesiones
            .Include(s => s.Usuario)
            .FirstOrDefault(s => s.Token == token && s.ExpiraEn > DateTime.Now);

        return sesion?.Usuario;
    }

    protected IActionResult ErrorNoAutorizado()
    {
        return StatusCode(401, new { mensaje = "Debes iniciar sesión." });
    }

    protected IActionResult ErrorSoloAdmin()
    {
        return StatusCode(403, new { mensaje = "Solo el administrador puede hacer esto." });
    }
}
