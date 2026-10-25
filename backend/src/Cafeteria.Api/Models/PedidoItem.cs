using System.Text.Json.Serialization;

namespace Cafeteria.Api.Models;

public class PedidoItem
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    [JsonIgnore]
    public Pedido Pedido { get; set; }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public string Notas { get; set; } = "";
}
