using Azure.Core;
using LowYurt3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;

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
                TempData["Error"] = "Debe ingresar correo y contraseña.";
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

            TempData["Error"] = "Credenciales incorrectas.";
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


        //////////////////////////////////////////////////////////////////////////////////////
        // Acción para editar perfil
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
        // GET: /Account/EditProfile
        [Authorize]
        public IActionResult EditProfile()
        {
            string currentUserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Login");
            }

            string rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (rol == "Cliente")
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Email == currentUserName);
                if (cliente == null)
                    return NotFound();

                return View(cliente);
            }
            else if (rol == "Admin")
            {
                var admin = _context.Admins.FirstOrDefault(a => a.CorreoAdmin == currentUserName);
                if (admin == null)
                    return NotFound();

                return View("EditAdmin", admin); // vista separada si deseas
            }

            return Unauthorized();
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Cliente model)
        {
            string currentUserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUserName))
                return RedirectToAction("Login");

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == currentUserName);
            if (cliente == null)
                return NotFound();

            cliente.Nombre = model.Nombre;
            cliente.SegundoNombre = model.SegundoNombre;
            cliente.Apellido = model.Apellido;
            cliente.SegundoApellido = model.SegundoApellido;
            cliente.Telefono = model.Telefono;
            cliente.Direccion = model.Direccion;
            cliente.Email = model.Email;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "✅ Tus datos se actualizaron correctamente.";
            return RedirectToAction("Profile");
        }

        ////////////////////////////////////////////////
        //Cambiar Contraseña
        ////////////////////////////////////////////////

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            string currentUserEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                TempData["Error"] = "Debe llenar todos los campos.";
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "La nueva contraseña y su confirmación no coinciden.";
                return View();
            }

            string email = User.Identity?.Name;
            if (email == null)
            {
                return RedirectToAction("Login");
            }

            // Buscar cliente en base de datos
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
            if (cliente == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return View();
            }

            // Hashear contraseña actual ingresada
            byte[] currentHash = HashPasswordToBytes(CurrentPassword);

            // Verificar si coincide con la almacenada
            if (!cliente.Contraseña.SequenceEqual(currentHash))
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return View();
            }

            //  Hashear nueva contraseña
            byte[] newHash = HashPasswordToBytes(NewPassword);

            //  Verificar si la nueva contraseña es igual a la anterior
            if (cliente.Contraseña.SequenceEqual(newHash))
            {
                TempData["Error"] = "La nueva contraseña no puede ser igual a la actual.";
                return View();
            }
            //Se actualiza la contraseña
            cliente.Contraseña = newHash;

            // Guardar cambios en la base de datos
            _context.Update(cliente);
            await _context.SaveChangesAsync();

            // Cerrar sesión para que vuelva a iniciar
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Mensaje"] = "Contraseña actualizada correctamente. Inicia sesión nuevamente.";

            return RedirectToAction("Login", "Account");
        }




        ///////////////////////////////////////////////////////
        /////Olvido Contraseña
        ///////////////////////////////////////////////////

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Debe ingresar un correo.";
                return View();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
            if (cliente == null)
            {
                TempData["Error"] = "No existe una cuenta con este correo.";
                return View();
            }

            // Generar token único
            cliente.ResetToken = Guid.NewGuid().ToString();
            cliente.TokenExpiration = DateTime.Now.AddHours(1); // 1 hora de validez

            await _context.SaveChangesAsync();

            // Crear enlace
            string resetLink = Url.Action("ResetPassword", "Account", new { token = cliente.ResetToken }, Request.Scheme);

            // Enviar correo
            await EnviarCorreoAsync(email, "Restablecer contraseña",
                $"Haz clic en el siguiente enlace para restablecer tu contraseña:<br><a href='{resetLink}'>Restablecer Contraseña</a>");

            TempData["Mensaje"] = "Se ha enviado un enlace de recuperación a tu correo.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ResetToken == token && c.TokenExpiration > DateTime.Now);
            if (cliente == null)
            {
                TempData["Error"] = "El enlace no es válido o ha expirado.";
                return View();
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                ViewBag.Token = token;
                return View();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ResetToken == token && c.TokenExpiration > DateTime.Now);
            if (cliente == null)
            {
                TempData["Error"] = "El enlace no es válido o ha expirado.";
                return View();
            }

            // Evitar que use la misma contraseña
            var newHash = HashPasswordToBytes(NewPassword);
            if (cliente.Contraseña.SequenceEqual(newHash))
            {
                TempData["Error"] = "La nueva contraseña no puede ser igual a la anterior.";
                ViewBag.Token = token;
                return View();
            }

            // Actualizar contraseña
            cliente.Contraseña = newHash;
            cliente.ResetToken = null;
            cliente.TokenExpiration = null;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Contraseña restablecida correctamente. Ya puedes iniciar sesión.";
            return View();
        }

        ///////
        ///Enviar correo
        //////////////////////////////

        private async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            string remitente = "andrexsenterprises@gmail.com"; // tu correo
            string clave = "mzcd jqnw revb rdaz";

            var mensaje = new MailMessage();
            mensaje.From = new MailAddress(remitente, "LowYurt Soporte");
            mensaje.To.Add(destinatario);
            mensaje.Subject = asunto;
            mensaje.Body = cuerpoHtml;
            mensaje.IsBodyHtml = true;

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(remitente, clave);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mensaje);
            }
        }

    }
}