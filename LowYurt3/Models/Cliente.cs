using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LowYurt3.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? SegundoNombre { get; set; }

    public string Apellido { get; set; } = null!;

    public string? SegundoApellido { get; set; }

    public long Telefono { get; set; }

    public string Direccion { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[]? Contraseña { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Debe ingresar una contraseña.")]
    [DataType(DataType.Password)]
    public string? ContraseñaTexto { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
