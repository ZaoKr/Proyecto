using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControladorBase
{
    private readonly PasswordHasher<Usuario> hasher;

    public AuthController(CafeteriaContext db, PasswordHasher<Usuario> hasher) : base(db)
    {
        this.hasher = hasher;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroRequest request)
    {
        string nombre = (request.Nombre ?? "").Trim();
        string email = (request.Email ?? "").Trim().ToLower();
        string password = request.Password ?? "";

        if (nombre.Length < 2)
        {
            return BadRequest(new { mensaje = "Escribe tu nombre." });
        }

        if (!email.Contains("@") || !email.Contains("."))
        {
            return BadRequest(new { mensaje = "Escribe un correo válido." });
        }

        if (password.Length < 6)
        {
            return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
        }

        bool existe = await Db.Usuarios.AnyAsync(u => u.Email == email);

        if (existe)
        {
            return Conflict(new { mensaje = "Ya existe una cuenta con ese correo." });
        }

        Usuario usuario = new Usuario
        {
            Nombre = nombre,
            Email = email,
            Rol = "cliente"
        };

        usuario.PasswordHash = hasher.HashPassword(usuario, password);

        Db.Usuarios.Add(usuario);
        await Db.SaveChangesAsync();

        Sesion sesion = CrearSesion(usuario);
        Db.Sesiones.Add(sesion);
        await Db.SaveChangesAsync();

        return Ok(new
        {
            token = sesion.Token,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        string email = (request.Email ?? "").Trim().ToLower();
        string password = request.Password ?? "";

        Usuario usuario = await Db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
        }

        PasswordVerificationResult resultado =
            hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
        }

        List<Sesion> vencidas = await Db.Sesiones
            .Where(s => s.ExpiraEn <= DateTime.Now)
            .ToListAsync();

        Db.Sesiones.RemoveRange(vencidas);

        Sesion sesion = CrearSesion(usuario);
        Db.Sesiones.Add(sesion);
        await Db.SaveChangesAsync();

        return Ok(new
        {
            token = sesion.Token,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol
            }
        });
    }

    [HttpGet("yo")]
    public IActionResult Yo()
    {
        Usuario usuario = UsuarioActual();

        if (usuario == null)
        {
            return ErrorNoAutorizado();
        }

        return Ok(new
        {
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Rol
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string autorizacion = Request.Headers.Authorization.ToString();

        if (!autorizacion.StartsWith("Bearer "))
        {
            return Ok(new { mensaje = "Sesión cerrada." });
        }

        string token = autorizacion.Substring("Bearer ".Length).Trim();

        Sesion sesion = await Db.Sesiones.FirstOrDefaultAsync(s => s.Token == token);

        if (sesion != null)
        {
            Db.Sesiones.Remove(sesion);
            await Db.SaveChangesAsync();
        }

        return Ok(new { mensaje = "Sesión cerrada." });
    }

    private static Sesion CrearSesion(Usuario usuario)
    {
        return new Sesion
        {
            Token = Guid.NewGuid().ToString("N"),
            UsuarioId = usuario.Id,
            ExpiraEn = DateTime.Now.AddDays(7)
        };
    }
}

public class RegistroRequest
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
