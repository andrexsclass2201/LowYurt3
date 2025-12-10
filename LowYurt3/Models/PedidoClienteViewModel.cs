namespace LowYurt3.Models
    {
        public class MisPedidosViewModel
        {
            public int IdPedido { get; set; }
            public DateTime? FechaCompra { get; set; }
            public int CantidadProductos { get; set; }
            public int ValorTotal { get; set; }

            public List<MisPedidosProductoViewModel> Productos { get; set; }
        }

        public class MisPedidosProductoViewModel
        {
            public int IdProducto { get; set; }
            public string NombreProducto { get; set; }
            public int Cantidad { get; set; }
            public int ValorUnitario { get; set; }
        }
    }
