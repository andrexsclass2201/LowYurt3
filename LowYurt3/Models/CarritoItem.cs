namespace LowYurt.Models
{
    public class CarritoItem
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
