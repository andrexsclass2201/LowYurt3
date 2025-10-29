using System;
using System.Collections.Generic;

namespace LowYurt3.Models
{
    public class UserProfileViewModel
    {
        public string Rol { get; set; }  // "Admin" o "Cliente"
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? Apellido { get; set; }
        public string? SegundoApellido { get; set; }
        public long Telefono { get; set; }
        public string Email { get; set; }
        public string? Direccion { get; set; }  // Nullable, solo para Cliente
        // Agrega más propiedades si tienes campos adicionales en tus tablas
    }
}