using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models;

[Table("Pedido")]
public partial class Pedido
{
    public int IdPedido { get; set; }

    public DateTime? FechaCompra { get; set; }

    public int CantidadProductos { get; set; }

    public int ValorTotal { get; set; }

    public int IdCliente { get; set; }

    public Cliente Cliente { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
