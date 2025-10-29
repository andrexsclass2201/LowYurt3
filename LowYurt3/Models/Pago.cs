using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public string? MetodoPago { get; set; } = "Mercado Pago";

    public int? ValorEnvio { get; set; } = 5000;

    public int PagoTotal { get; set; }

    public int IdPedido { get; set; }
    public Pedido Pedido { get; set; }

}
