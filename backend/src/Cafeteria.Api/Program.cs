using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string puerto = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrWhiteSpace(puerto))
{
    builder.WebHost.UseUrls("http://0.0.0.0:" + puerto);
}

builder.Services.AddControllers();

string rutaBaseDatos = Environment.GetEnvironmentVariable("DB_PATH");

if (string.IsNullOrWhiteSpace(rutaBaseDatos))
{
    rutaBaseDatos = Path.Combine(builder.Environment.ContentRootPath, "cafeteria.db");
}
else
{
    string carpeta = Path.GetDirectoryName(rutaBaseDatos);

    if (!string.IsNullOrWhiteSpace(carpeta))
    {
        Directory.CreateDirectory(carpeta);
    }

    if (!File.Exists(rutaBaseDatos))
    {
        string baseIncluida = Path.Combine(builder.Environment.ContentRootPath, "cafeteria.db");

        if (File.Exists(baseIncluida) && !string.Equals(baseIncluida, rutaBaseDatos, StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(baseIncluida, rutaBaseDatos);
        }
    }
}

builder.Services.AddDbContext<CafeteriaContext>(options =>
    options.UseSqlite("Data Source=" + rutaBaseDatos));

builder.Services.AddSingleton<PasswordHasher<Usuario>>();

string origenesPermitidos = Environment.GetEnvironmentVariable("FRONTEND_ORIGINS");

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAngular", policy =>
    {
        if (origenesPermitidos == "*")
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else if (string.IsNullOrWhiteSpace(origenesPermitidos))
        {
            policy
                .SetIsOriginAllowed(origen =>
                {
                    return Uri.TryCreate(origen, UriKind.Absolute, out Uri uri)
                        && (uri.Host == "localhost" || uri.Host == "127.0.0.1");
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            string[] origenes = origenesPermitidos.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            policy
                .WithOrigins(origenes)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    CafeteriaContext db = scope.ServiceProvider.GetRequiredService<CafeteriaContext>();
    db.Database.EnsureCreated();
    DatosIniciales.Cargar(db);
}

app.UseCors("FrontendAngular");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    api = "Cafetería Aroma",
    mensaje = "La API está funcionando.",
    baseDatos = "SQLite",
    estado = "/api/estado"
}));

app.MapGet("/api/estado", () => Results.Ok(new
{
    estado = "ok",
    baseDatos = "SQLite",
    hora = DateTime.Now
}));

app.MapFallbackToFile("index.html");

app.Run();
