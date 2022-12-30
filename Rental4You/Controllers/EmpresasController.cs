using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? NomeEmpresa, string? Estado = "Todos")
        {
            var estados = new List<String>();
            estados.Add("Todos");
            estados.Add("Ativo");
            estados.Add("Inativo");
            ViewData["Estados"] = new SelectList(estados);

            if (Estado == null || Estado.Equals("Todos") && NomeEmpresa == null)
            {
                return View(await _context.Empresas.ToListAsync());
            }
            var estado = Estado.Equals("Ativo") ? true : false;
            if (NomeEmpresa != null)
            {
                return View(await _context.Empresas.Where(e => e.EstadoSubscricao == estado &&
                    e.Nome.ToLower().Contains(NomeEmpresa.ToLower())).ToListAsync());
            } else
            {
                return View(await _context.Empresas.Where(e => e.EstadoSubscricao == estado).ToListAsync());
            }
        }


        // GET: Empresas/Details/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,EstadoSubscricao,Avalicao")] Empresa empresa)
        {
            int NumberOfUsers = _userManager.Users.Count() + 1;
            ApplicationUser user = new ApplicationUser();
            user.UserName = String.Format("Gestor{}@{}.com", NumberOfUsers, empresa.Nome);
            var result = await _userManager.CreateAsync(user, "Is3C..00");

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Gestor");
                ModelState.Remove(nameof(Empresa.Gestores));
                if (ModelState.IsValid)
                {
                    Gestor gestor = new Gestor();
                    gestor.Nome = "Gestor" + NumberOfUsers;
                    gestor.ApplicationUser = user;
                    gestor.ApplicationUser.EmailConfirmed = true;
                    gestor.ApplicationUser.Ativo = true;
                    var funcionario = new Funcionario();
                    funcionario.ApplicationUser = user;
                    funcionario.Empresa = empresa;
                    funcionario.EmpresaId = empresa.Id;
                    funcionario.Nome = gestor.Nome.Trim();
                    _context.Add(empresa);
                    await _context.SaveChangesAsync();
                    gestor.EmpresaId = _context.Empresas.OrderBy(e => e.Id).Last().Id;
                    _context.Add(gestor);
                    _context.Add(funcionario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                    var gestores = _context.Gestores.Include(g => g.ApplicationUser).Where(g => g.EmpresaId == empresa.Id).ToList();

                    foreach(var gestor in gestores)
                    {
                        var user = _userManager.Users.Where(u => u.Id == gestor.ApplicationUser.Id).FirstOrDefault();
                        if (user != null)
                        {
                            _context.Gestores.RemoveRange(_context.Gestores.Where(g => g.Id == gestor.Id));
                            _context.Users.Remove(user);
                        }
                       
                    }
    
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
