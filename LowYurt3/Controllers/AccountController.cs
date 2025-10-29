using LowYurt3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LowYurt3.Controllers
{
    public class AccountController : Controller
    {
        private readonly LowYurtContext _context;

        public AccountController(LowYurtContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string contraseña)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.Error = "Debe ingresar correo y contraseña.";
                return View();
            }

            // 🔑 Convertir la contraseña ingresada a hash SHA256 en bytes
            byte[] hashedPassword = HashPasswordToBytes(contraseña);

            // Buscar primero en Admin
            var admin = await _context.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.CorreoAdmin == email && a.ContraseñaAdmin == hashedPassword);

            if (admin != null)
            {
                HttpContext.Session.SetString("UserEmail", admin.CorreoAdmin);
                HttpContext.Session.SetString("UserRole", "Admin");
                await SignInUser(email, "Admin");
                return RedirectToAction("Index", "Productos");
            }

            // Buscar en Cliente
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == email && c.Contraseña == hashedPassword);

            if (cliente != null)
            {
                HttpContext.Session.SetString("UserEmail", cliente.Email);
                HttpContext.Session.SetString("Rol", "Cliente");
                await SignInUser(email, "Cliente");
                return RedirectToAction("Index", "Productos");
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View();

        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ======================
        // Helpers
        // ======================

        private async Task SignInUser(string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantiene sesión
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Expira en 1 hora
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private byte[] HashPasswordToBytes(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Acción para el perfil (actualizada para usar Claims y evitar nulls)
        [Authorize]
        public IActionResult Profile()
        {
            string currentUserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Login");
            }

            string rol = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(rol))
            {
                return RedirectToAction("Login");
            }

            UserProfileViewModel model = null;

            if (rol == "Admin")
            {
                var admin = _context.Admins.FirstOrDefault(a => a.CorreoAdmin == currentUserName);
                if (admin == null)
                {
                    return NotFound();
                }
                model = new UserProfileViewModel
                {
                    Rol = "Admin",
                    Id = admin.IdAdmin,
                    Nombre = admin.NombreAdmin,
                    Email = admin.CorreoAdmin
                    // Agrega más si tienes campos extras
                };
            }
            else if (rol == "Cliente")
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Email == currentUserName);
                if (cliente == null)
                {
                    return NotFound();
                }
                model = new UserProfileViewModel
                {
                    Rol = "Cliente",
                    Id = cliente.IdCliente,
                    SegundoNombre = cliente.SegundoNombre,
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    SegundoApellido = cliente.SegundoApellido,
                    Telefono = cliente.Telefono,
                    Email = cliente.Email,
                    Direccion = cliente.Direccion
                    // Agrega más si tienes campos extras
                };
            }
            else
            {
                return Unauthorized();
            }

            if (model == null)
            {
                return NotFound("Perfil no encontrado.");
            }

            return View(model);
        }
        public IActionResult Contrasena()
        {
            return View();
        }
    }
}