using Cafeteria.Api.Data;

namespace Cafeteria.Api.Models;

public class Pedido
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public string Tipo { get; set; } = "ParaLlevar";

    public int? NumeroMesa { get; set; }

    public string MetodoPago { get; set; } = "Efectivo";

    public string Estado { get; set; } = "Pendiente";

    public decimal Total { get; set; }
    public string Notas { get; set; } = "";
    public DateTime CreadoEn { get; set; } = Reloj.Ahora();

    public List<PedidoItem> Items { get; set; } = new List<PedidoItem>();
}
