using LowYurt3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LowYurt3.Controllers
{
    public class ProductosController : Controller
    {
        private readonly LowYurtContext _context;

        public ProductosController(LowYurtContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(int? categoriaId)
        {
            // Obtener categorías
            var categorias = await _context.Categoria.ToListAsync();
            ViewBag.Categorias = categorias;

            // Filtrar productos
            var productos = _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .AsQueryable();

            if (categoriaId.HasValue)
            {
                productos = productos.Where(p => p.IdCategoria == categoriaId);
            }

            return View(await productos.ToListAsync());
        }


        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "NombreCategoria");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("NombreProducto,Descripcion,ValorUnitario,ImagenProducto,IdCategoria")] Producto producto)
        {
            ModelState.Remove(nameof(producto.IdCategoriaNavigation));
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje1"] = "Producto creado exitosamente.";
                return RedirectToAction();
            }
            else
            {
                // 👀 Para depuración: muestra los errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Errors = errors;
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "NombreCategoria", producto.IdCategoria);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "NombreCategoria", producto.IdCategoria);
            return View(producto);

        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,NombreProducto,Descripcion,ValorUnitario,ImagenProducto,IdCategoria")] Producto producto)
        {
            ModelState.Remove(nameof(producto.IdCategoriaNavigation));
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    TempData["Mensaje2"] = "Producto editado exitosamente.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction();
            }
            else
            {
                // 👀 Para depuración: muestra los errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Errors = errors;
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categoria, "IdCategoria", "NombreCategoria", producto.IdCategoria);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}
