using System.Text.Json.Serialization;

namespace Cafeteria.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";

    [JsonIgnore]
    public List<Producto> Productos { get; set; } = new List<Producto>();
}
