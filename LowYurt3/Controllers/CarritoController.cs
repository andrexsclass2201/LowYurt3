using LowYurt.Models;
using LowYurt3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System.Linq;
using LowYurt.Services;


namespace LowYurt.Controllers
{
    public class CarritoController : Controller
    {
        private readonly LowYurtContext _context;
        private readonly IConfiguration _configuration;
        private readonly PayPalService _paypalService;


        public CarritoController(LowYurtContext context, IConfiguration configuration, PayPalService paypalService)
        {
            _context = context;
            _configuration = configuration;
            _paypalService = paypalService;
        }

        // ----------------------------- CARRITO -----------------------------

        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoJson))
                return new List<CarritoItem>();

            return JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson)!;
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            var carritoJson = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString("Carrito", carritoJson);
        }

        public IActionResult Agregar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null)
                return NotFound();

            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(p => p.IdProducto == id);

            if (item == null)
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = producto.IdProducto,
                    NombreProducto = producto.NombreProducto,
                    Precio = producto.ValorUnitario,
                    Cantidad = 1
                });
            }
            else
            {
                item.Cantidad++;
            }

            GuardarCarrito(carrito);
            TempData["Mensaje"] = $"{producto.NombreProducto} se agregó al carrito.";

            return RedirectToAction("Index", "Productos");
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            // Subtotal sin envío
            decimal subtotal = carrito.Sum(i => i.Precio * i.Cantidad);

            // Valor fijo de envío
            decimal valorEnvio = 5000;

            // Total con envío
            decimal total = subtotal + valorEnvio;

            ViewBag.Subtotal = subtotal;
            ViewBag.ValorEnvio = valorEnvio;
            ViewBag.Total = total;

            return View(carrito);
        }

        public IActionResult Eliminar(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(p => p.IdProducto == id);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove("Carrito");
            return RedirectToAction("Index");
        }
        //----------------------------- ACTUALIZAR CANTIDADES -----------------------------
        public IActionResult AumentarCantidad(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(p => p.IdProducto == id);
            if (item != null)
            {
                item.Cantidad++;
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        public IActionResult DisminuirCantidad(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(p => p.IdProducto == id);
            if (item != null)
            {
                if (item.Cantidad > 1)
                    item.Cantidad--;
                else
                    carrito.Remove(item);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        // ----------------------------- PAGO Mercado Pago -----------------------------

        [Authorize]
        public async Task<IActionResult> Pagar()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Mensaje"] = "Debes iniciar sesión para proceder al pago.";
                return RedirectToAction("Login", "Account");
            }

            var carrito = ObtenerCarrito();
            if (carrito == null || !carrito.Any())
            {
                TempData["Mensaje"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }
            // Buscar cliente
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == userEmail);
            if (cliente == null)
            {
                TempData["Mensaje"] = "No se encontró el cliente con el correo asociado.";
                return RedirectToAction("Login", "Account");
            }

            var pedido = new Pedido
            {
                CantidadProductos = carrito.Sum(p => p.Cantidad),
                ValorTotal = carrito.Sum(p => p.Precio * p.Cantidad),
                IdCliente = cliente.IdCliente
            };
            _context.Pedidos.Add(pedido);
            _context.SaveChanges(); // Guardar para obtener Id_Pedido

            // Crear los detalles del pedido
            foreach (var item in carrito)
            {
                var detalle = new PedidoDetalle
                {
                    IdPedido = pedido.IdPedido,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                };
                _context.PedidoDetalles.Add(detalle);
            }
            _context.SaveChanges();

            // Crear el registro de pago
            var pago = new Pago
            {
                MetodoPago = "MercadoPago",
                ValorEnvio = 5000,
                PagoTotal = pedido.ValorTotal + 5000,
                IdPedido = pedido.IdPedido
            };
            _context.Pagos.Add(pago);
            _context.SaveChanges();

            // Verifica AccessToken
            string accessToken = _configuration["MercadoPago:AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Mensaje"] = "⚠️ No se encontró el AccessToken de Mercado Pago.";
                Console.WriteLine("❌ AccessToken no configurado");
                return RedirectToAction("Index");
            }

            try
            {
                // Inicializa SDK
                MercadoPagoConfig.AccessToken = accessToken;

                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                Console.WriteLine($"🌐 Base URL: {baseUrl}");

                // Crea los ítems
                var items = carrito.Select(c => new PreferenceItemRequest
                {
                    Title = c.NombreProducto ?? "Producto",
                    Quantity = c.Cantidad,
                    UnitPrice = (decimal)c.Precio,
                    CurrencyId = "COP"
                }).ToList();

                // Agrega el costo fijo de envío como un ítem adicional
                items.Add(new PreferenceItemRequest
                {
                    Title = "Costo de envío",
                    Quantity = 1,
                    UnitPrice = 5000m, // Valor fijo de envío
                    CurrencyId = "COP"
                });

                var preferenceRequest = new PreferenceRequest
                {
                    Items = items,
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://www.mercadopago.com.co",
                        Failure = "https://www.mercadopago.com.co",
                        Pending = "https://www.mercadopago.com.co"
                    },
                    AutoReturn = "approved"
                };

                Console.WriteLine("🟢 Enviando preferencia a Mercado Pago...");

                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(preferenceRequest);

                Console.WriteLine("✅ Preferencia creada correctamente");
                Console.WriteLine($"🔗 URL de pago: {preference.InitPoint}");

                // 🔹 Redirige directamente a Mercado Pago
                Response.Redirect(preference.SandboxInitPoint, false);
                return new EmptyResult();

            }

            catch (MercadoPago.Error.MercadoPagoApiException ex)
            {
                Console.WriteLine("❌ Error Mercado Pago: " + ex.ApiResponse.Content);
                TempData["Mensaje"] = "Error Mercado Pago: " + ex.ApiResponse.Content;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Error general: " + ex.Message);
                TempData["Mensaje"] = "Error inesperado: " + ex.Message;
                return RedirectToAction("Index");
            }

        }

        // ----------------------------- PAGO PAYPAL -----------------------------
        [Authorize]
        public async Task<IActionResult> PagarConPayPal()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Mensaje"] = "Debes iniciar sesión para proceder al pago.";
                return RedirectToAction("Login", "Account");
            }

            var carrito = ObtenerCarrito();
            if (carrito == null || !carrito.Any())
            {
                TempData["Mensaje"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == userEmail);
            if (cliente == null)
            {
                TempData["Mensaje"] = "No se encontró el cliente con el correo asociado.";
                return RedirectToAction("Login", "Account");
            }

            var pedido = new Pedido
            {
                CantidadProductos = carrito.Sum(p => p.Cantidad),
                ValorTotal = carrito.Sum(p => p.Precio * p.Cantidad),
                IdCliente = cliente.IdCliente
            };
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            foreach (var item in carrito)
            {
                _context.PedidoDetalles.Add(new PedidoDetalle
                {
                    IdPedido = pedido.IdPedido,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                });
            }
            _context.SaveChanges();

            decimal total = pedido.ValorTotal + 5000;
            var returnUrl = Url.Action("PagoExitosoPayPal", "Carrito", new { idPedido = pedido.IdPedido }, Request.Scheme);
            var cancelUrl = Url.Action("PagoCanceladoPayPal", "Carrito", null, Request.Scheme);
            var approvalUrl = await _paypalService.CreateOrderAsync(total, returnUrl, cancelUrl);

            return Redirect(approvalUrl);
        }

        public async Task<IActionResult> PagoExitosoPayPal(string token, int idPedido)
        {
            await _paypalService.CaptureOrderAsync(token);

            var pago = new Pago
            {
                MetodoPago = "PayPal",
                ValorEnvio = 5000,
                PagoTotal = _context.Pedidos.Find(idPedido).ValorTotal + 5000,
                IdPedido = idPedido
            };
            _context.Pagos.Add(pago);
            _context.SaveChanges();

            HttpContext.Session.Remove("Carrito");
            TempData["Mensaje"] = "✅ Pago realizado con éxito a través de PayPal.";

            return RedirectToAction("Index", "Productos");
        }

        public IActionResult PagoCanceladoPayPal()
        {
            TempData["Mensaje"] = "❌ Pago cancelado por el usuario.";
            return RedirectToAction("Index");
        }
    }
}
