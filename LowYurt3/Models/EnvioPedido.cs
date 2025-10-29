using System;
using System.Collections.Generic;

namespace LowYurt3.Models;

public partial class EnvioPedido
{
    public int IdEnvio { get; set; }

    public string? Direccion { get; set; }

    public int IdPedido { get; set; }

    public int IdCliente { get; set; }

    public int IdPago { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Pago IdPagoNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
