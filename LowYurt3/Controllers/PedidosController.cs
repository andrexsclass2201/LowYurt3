using LowYurt3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LowYurt3.Controllers
{
    public class PedidosController : Controller
    {
        private readonly LowYurtContext _context;

        public PedidosController(LowYurtContext context)
        {
            _context = context;
        }

        // GET: Pedidos
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Gestion()
        {
            // Consulta combinada como tu SQL original
            var pedidos = (from p in _context.Pedidos
                           join pd in _context.PedidoDetalles on p.IdPedido equals pd.IdPedido
                           join pr in _context.Productos on pd.IdProducto equals pr.IdProducto
                           join c in _context.Clientes on p.IdCliente equals c.IdCliente
                           select new LowYurt3.Models.PedidoAdminViewModel
                           {
                               IdPedido = p.IdPedido,
                               IdCliente = c.IdCliente,
                               NombreCliente = c.Nombre + " " + c.Apellido,
                               FechaCompra = p.FechaCompra,
                               IdProducto = pr.IdProducto,
                               NombreProducto = pr.NombreProducto,
                               Cantidad = pd.Cantidad ?? 0,
                               ValorUnitario = pr.ValorUnitario,
                               CantidadProductos = p.CantidadProductos,
                               ValorTotal = p.ValorTotal
                           })
               .OrderByDescending(p => p.FechaCompra)
               .ToList();

            return View(pedidos);
        }

        // GET: Pedidos/Detalles/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detalles(int id)
        {
            var pedido = await (
                from p in _context.Pedidos
                join pd in _context.PedidoDetalles on p.IdPedido equals pd.IdPedido
                join pr in _context.Productos on pd.IdProducto equals pr.IdProducto
                join c in _context.Clientes on p.IdCliente equals c.IdCliente
                where p.IdPedido == id
                select new PedidoAdminViewModel
                {
                    IdPedido = p.IdPedido,
                    IdCliente = c.IdCliente,
                    NombreCliente = c.Nombre + " " + c.Apellido,
                    FechaCompra = p.FechaCompra,
                    IdProducto = pr.IdProducto,
                    NombreProducto = pr.NombreProducto,
                    Cantidad = pd.Cantidad ?? 0,
                    ValorUnitario = pr.ValorUnitario,
                    CantidadProductos = p.CantidadProductos,
                    ValorTotal = p.ValorTotal
                })
                .ToListAsync();

            if (pedido == null || pedido.Count == 0)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] = "Pedido eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        /////Pedidos Cliente/////////
        /////////////////////////////////////////////////////////////////////

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisPedidos()
        {
            // Obtener email del usuario logueado
            string email = User.Identity?.Name;

            if (email == null)
                return RedirectToAction("Login", "Account");

            // Buscar cliente en BD
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            // Cargar los pedidos del cliente
            var pedidos = await _context.Pedidos
                .Where(p => p.IdCliente == cliente.IdCliente)
                .OrderByDescending(p => p.FechaCompra)
                .Select(p => new MisPedidosViewModel
                {
                    IdPedido = p.IdPedido,
                    FechaCompra = p.FechaCompra,
                    CantidadProductos = p.CantidadProductos,
                    ValorTotal = p.ValorTotal,
                    Productos = p.PedidoDetalles.Select(d => new MisPedidosProductoViewModel
                    {
                        IdProducto = d.IdProducto,
                        NombreProducto = d.Producto.NombreProducto,
                        Cantidad = d.Cantidad ?? 0,
                        ValorUnitario = d.Producto.ValorUnitario
                    }).ToList()
                })
                .ToListAsync();

            return View(pedidos);
        }

    }
}
