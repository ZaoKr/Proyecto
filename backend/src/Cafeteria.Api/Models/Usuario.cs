using System.Text.Json.Serialization;

namespace Cafeteria.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";

    [JsonIgnore]
    public string PasswordHash { get; set; } = "";

    public string Rol { get; set; } = "cliente";
}
