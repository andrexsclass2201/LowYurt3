using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models;

public partial class Producto
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdProducto { get; set; }

    public string? NombreProducto { get; set; }

    public string? Descripcion { get; set; }

    public int ValorUnitario { get; set; }

    public string? ImagenProducto { get; set; }

    public int IdCategoria { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
