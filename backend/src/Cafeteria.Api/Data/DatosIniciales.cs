using Cafeteria.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Cafeteria.Api.Data;

public static class DatosIniciales
{
    public static void Cargar(CafeteriaContext db)
    {
        if (db.Usuarios.Any())
        {
            return;
        }

        PasswordHasher<Usuario> hasher = new PasswordHasher<Usuario>();

        Usuario admin = new Usuario
        {
            Nombre = "Administrador",
            Email = "admin@cafeteria.com",
            Rol = "admin"
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        Usuario cliente = new Usuario
        {
            Nombre = "Cliente de prueba",
            Email = "cliente@cafeteria.com",
            Rol = "cliente"
        };
        cliente.PasswordHash = hasher.HashPassword(cliente, "Cliente123!");

        db.Usuarios.Add(admin);
        db.Usuarios.Add(cliente);
        db.SaveChanges();

        Categoria calientes = new Categoria { Nombre = "Bebidas calientes" };
        Categoria frias = new Categoria { Nombre = "Bebidas frías" };
        Categoria reposteria = new Categoria { Nombre = "Repostería" };
        Categoria comida = new Categoria { Nombre = "Comida" };

        db.Categorias.AddRange(calientes, frias, reposteria, comida);
        db.SaveChanges();

        db.Productos.AddRange(
            new Producto
            {
                Nombre = "Café americano",
                Descripcion = "Café negro recién pasado.",
                Precio = 6.00m,
                CategoriaId = calientes.Id,
                ImgUrl = "https://www.nescafe.com/es/sites/default/files/2023-09/1_desk.jpg"
            },
            new Producto
            {
                Nombre = "Cappuccino",
                Descripcion = "Espresso con leche vaporizada y espuma.",
                Precio = 9.50m,
                CategoriaId = calientes.Id
            },
            new Producto
            {
                Nombre = "Latte",
                Descripcion = "Espresso con leche cremosa.",
                Precio = 9.00m,
                CategoriaId = calientes.Id
            },
            new Producto
            {
                Nombre = "Chocolate caliente",
                Descripcion = "Chocolate con leche y un toque de canela.",
                Precio = 8.00m,
                CategoriaId = calientes.Id
            },
            new Producto
            {
                Nombre = "Té verde",
                Descripcion = "Té verde natural en hebras.",
                Precio = 5.00m,
                CategoriaId = calientes.Id
            },
            new Producto
            {
                Nombre = "Cold brew",
                Descripcion = "Café frío preparado lentamente.",
                Precio = 10.00m,
                CategoriaId = frias.Id
            },
            new Producto
            {
                Nombre = "Frappé de café",
                Descripcion = "Café frío con hielo y crema.",
                Precio = 12.50m,
                CategoriaId = frias.Id
            },
            new Producto
            {
                Nombre = "Limonada frozen",
                Descripcion = "Limón, hielo y un poco de azúcar.",
                Precio = 9.00m,
                CategoriaId = frias.Id
            },
            new Producto
            {
                Nombre = "Jugo de naranja",
                Descripcion = "Jugo natural de naranja.",
                Precio = 8.00m,
                CategoriaId = frias.Id
            },
            new Producto
            {
                Nombre = "Croissant",
                Descripcion = "Croissant de mantequilla.",
                Precio = 6.50m,
                CategoriaId = reposteria.Id
            },
            new Producto
            {
                Nombre = "Cheesecake",
                Descripcion = "Porción de cheesecake con frutos rojos.",
                Precio = 11.00m,
                CategoriaId = reposteria.Id
            },
            new Producto
            {
                Nombre = "Muffin de arándanos",
                Descripcion = "Muffin esponjoso con arándanos.",
                Precio = 7.50m,
                CategoriaId = reposteria.Id
            },
            new Producto
            {
                Nombre = "Brownie",
                Descripcion = "Brownie de chocolate con nueces.",
                Precio = 6.00m,
                CategoriaId = reposteria.Id
            },
            new Producto
            {
                Nombre = "Sándwich de pollo",
                Descripcion = "Pan ciabatta con pollo y vegetales.",
                Precio = 13.00m,
                CategoriaId = comida.Id
            },
            new Producto
            {
                Nombre = "Empanada de carne",
                Descripcion = "Empanada horneada de carne.",
                Precio = 7.00m,
                CategoriaId = comida.Id
            },
            new Producto
            {
                Nombre = "Ensalada César",
                Descripcion = "Lechuga, pollo, queso y aderezo César.",
                Precio = 14.00m,
                CategoriaId = comida.Id
            }
        );

        db.SaveChanges();
    }
}
