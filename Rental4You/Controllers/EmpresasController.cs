using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empresas
        public async Task<IActionResult> Index(bool? estado)
        {
            if(estado == null)
                return View(await _context.Empresas.ToListAsync());
            return View(await _context.Empresas.Where(e => e.EstadoSubscricao == estado).ToListAsync());
        }

        //POST: Empresas
        [HttpPost]
        public async Task<IActionResult> Index(string TextoAPesquisar)
        {
            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
                return View(await _context.Empresas.ToListAsync());
            return View(await _context.Empresas.Where(e => e.Nome.ToLower().Contains(TextoAPesquisar.ToLower())).ToListAsync());
        }

        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,EstadoSubscricao,Avalicao")] Empresa empresa)
        {
            int NumberOfGestores = _context.Gestores.Count() + 1;
            ApplicationUser user = new ApplicationUser();
            user.UserName = "Gestor" + NumberOfGestores + "@" + empresa.Nome + ".com";
            var result = await _userManager.CreateAsync(user, "Is3C..00");

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Gestor");
                ModelState.Remove(nameof(Empresa.Gestores));
                if (ModelState.IsValid)
                {
                    ApplicationUser _user = _userManager.Users.OrderBy(u => u.Id).Last();
                    Gestor gestor = new Gestor();
                    gestor.Nome = user.UserName;
                    gestor.ApplicationUser = _user;
                    _context.Add(empresa);
                    await _context.SaveChangesAsync();
                    gestor.EmpresaId = _context.Empresas.OrderBy(e => e.Id).Last().Id;
                    _context.Add(gestor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,EstadoSubscricao,Avalicao")] Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(Empresa.Gestores));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Id))
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
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Empresas'  is null.");
            }
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa != null)
            {
                if(_context.Veiculos.Count(v => v.EmpresaId == empresa.Id) <= 0)
                {
                    var gestores = _context.Gestores.Include(g => g.ApplicationUser)
                        .Where(g => g.EmpresaId == empresa.Id).ToList();
                    foreach(var gestor in gestores)
                    {
                        var user = _userManager.Users.Where(u => u.Id == gestor.ApplicationUser.Id).FirstOrDefault();
                        if(user != null)
                            await _userManager.DeleteAsync(user);
                    }
                    _context.Gestores.RemoveRange(gestores);
                    _context.Empresas.Remove(empresa);
                } else
                {
                    return Problem("A empresa tem veículos associados");
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaExists(int id)
        {
          return _context.Empresas.Any(e => e.Id == id);
        }
    }
}
