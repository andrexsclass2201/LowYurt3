using System;
using System.Collections.Generic;

namespace LowYurt3.Models;

public partial class Categorium
{
    public int IdCategoria { get; set; }

    public string? NombreCategoria { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
