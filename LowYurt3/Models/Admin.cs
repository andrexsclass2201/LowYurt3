using System;
using System.Collections.Generic;

namespace LowYurt3.Models;

public partial class Admin
{
    public int IdAdmin { get; set; }

    public string NombreAdmin { get; set; } = null!;

    public string CorreoAdmin { get; set; } = null!;

    public byte[] ContraseñaAdmin { get; set; } = null!;
}
