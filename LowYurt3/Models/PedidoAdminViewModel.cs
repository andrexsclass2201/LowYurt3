using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models
{
    public class PedidoAdminViewModel
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }

        public string NombreCliente { get; set; }
        public DateTime? FechaCompra { get; set; }

        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }

        public int Cantidad { get; set; }

        public int ValorUnitario { get; set; }

        public int CantidadProductos { get; set; }

        public int ValorTotal { get; set; }
    }
}
