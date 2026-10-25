using Cafeteria.Api.Data;
using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string rutaBaseDatos = Path.Combine(builder.Environment.ContentRootPath, "cafeteria.db");

builder.Services.AddDbContext<CafeteriaContext>(options =>
    options.UseSqlite("Data Source=" + rutaBaseDatos));

builder.Services.AddSingleton<PasswordHasher<Usuario>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendAngular", policy =>
    {
        policy
            .SetIsOriginAllowed(origen =>
            {
                return Uri.TryCreate(origen, UriKind.Absolute, out Uri uri)
                    && (uri.Host == "localhost" || uri.Host == "127.0.0.1");
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
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

app.Run();
