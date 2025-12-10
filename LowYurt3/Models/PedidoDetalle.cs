using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models;

[Table("Pedido_Detalle")]
public partial class PedidoDetalle
{
    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public int? Cantidad { get; set; }

    // Navegaciones CORRECTAS
    public virtual Pedido Pedido { get; set; }
    public virtual Producto Producto { get; set; }
}
