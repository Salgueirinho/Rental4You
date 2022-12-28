using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;

namespace Rental4You.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public VeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Empresa? getEmpresa()
        {
            var userId = _userManager.GetUserId(User);
            var gestor = _context.Gestores.Include(g => g.ApplicationUser).Include(g => g.Empresa)
                .Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            Empresa? empresa = null;
            if (gestor == null)
            {
                userId = _userManager.GetUserId(User);
                var funcionario = _context.Funcionarios.Include(g => g.ApplicationUser).Include(g => g.Empresa)
                    .Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
                if (funcionario == null)
                    return null;
                empresa = funcionario.Empresa;
            }
            else
            {
                empresa = gestor.Empresa;
            }
            return empresa;
        }

        // GET: Veiculos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa);
            var empresa = getEmpresa();
            if(empresa == null)
                return NotFound();
            ViewBag.NomeEmpresa = empresa.Nome;
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Veiculos/Create
        public IActionResult Create()
        {
            var empresa = getEmpresa();

            if (empresa == null)
                return NotFound();

            
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e=>e.Id == empresa.Id), "Id", "Nome");
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Marca,Modelo,CategoriaId,NumeroLugares,Caixa,Disponivel,Custo,Danos,Kilometros,EmpresaId")] Veiculo veiculo)
        {
            ModelState.Remove(nameof(Veiculo.Empresa));
            ModelState.Remove(nameof(Veiculo.Categoria));
            if (ModelState.IsValid)
            {
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var empresa = getEmpresa();

            if (empresa == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == empresa.Id), "Id", "Nome");
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Veiculos == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var empresa = getEmpresa();

            if (empresa == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == empresa.Id), "Id", "Nome");
            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,CategoriaId,NumeroLugares,Caixa,Disponivel,Custo,Danos,Kilometros,EmpresaId")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Veiculo.Empresa));
            ModelState.Remove(nameof(Veiculo.Categoria));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var empresa = getEmpresa();

            if (empresa == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == empresa.Id), "Id", "Nome");
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Veiculos == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.Categoria)
                .Include(v => v.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculos'  is null.");
            }
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null)
            {
                _context.Veiculos.Remove(veiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
          return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}
