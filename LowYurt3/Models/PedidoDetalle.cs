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
    public Pedido Pedido { get; set; } 
    public Producto Producto { get; set; } 
}
