using System.Text.Json.Serialization;

namespace Cafeteria.Api.Models;

public class Sesion
{
    public int Id { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiraEn { get; set; }

    public int UsuarioId { get; set; }

    [JsonIgnore]
    public Usuario Usuario { get; set; }
}
