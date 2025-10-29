using System.Diagnostics;
using LowYurt3.Models;
using Microsoft.AspNetCore.Mvc;

namespace LowYurt3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TerminosYCondiciones()
        {
            return View();
        }

        public IActionResult View1()
        {
            return View();
        }
        public IActionResult Carrito()
        {
            return View();
        }
        public IActionResult AcercaNosotros()
        {
            return View();
        }
        public IActionResult InicioSesion()
        {
            return View();
        }
        public IActionResult Registrarse()
        {
            return View();
        }
        public IActionResult CreateCliente()
        {
            return View();
        }
        public IActionResult Contrasena()
        {
            return View();
        }
        public IActionResult SobreNosotros()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
