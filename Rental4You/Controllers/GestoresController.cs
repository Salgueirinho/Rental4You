using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;

namespace Rental4You.Controllers
{
    public class GestoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public GestoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Gestores
        [Authorize(Roles = "Gestor, Admin")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var gestor = _context.Gestores.Include(g => g.ApplicationUser).Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            if (gestor == null)
                return (NotFound());
            var gestores = _context.Gestores.Include(g => g.Empresa).Include(g => g.ApplicationUser)
                .Where(g => g.EmpresaId == gestor.EmpresaId);
            return View(await gestores.ToListAsync());
        }

        // GET: Gestores/Create
        [Authorize(Roles = "Gestor, Admin")]
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            var gestor = _context.Gestores.Include(g => g.ApplicationUser).Include(g => g.Empresa)
                .Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            if (gestor == null)
                return NotFound();
            ViewBag.NomeEmpresa = gestor.Empresa.Nome;
            return View();
        }

        // POST: Gestores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor, Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Gestor gestor)
        {
            var userId = _userManager.GetUserId(User);
            var gestorTemp = _context.Gestores.Include(g => g.ApplicationUser).Include(g => g.Empresa)
                .Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            if (gestorTemp == null)
                return NotFound();
            var empresa = _context.Empresas.Where(e => e.Id == gestorTemp.EmpresaId).FirstOrDefault();
            if (empresa == null)
                return NotFound();
            var name = gestor.Nome.Replace(" ", "");
            ApplicationUser user = new ApplicationUser();
            user.EmailConfirmed = true;
            user.Ativo = true;
            user.UserName = name + "@" + empresa.Nome + ".com";
            user.Email = user.UserName;
            var result = await _userManager.CreateAsync(user, "Is3C..00");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Gestor");
                ModelState.Remove(nameof(Gestor.Empresa));
                ModelState.Remove(nameof(Gestor.ApplicationUser));
                if (ModelState.IsValid)
                {
                    var funcionario = new Funcionario();
                    funcionario.ApplicationUser = user;
                    funcionario.Empresa = empresa;
                    funcionario.EmpresaId = empresa.Id;
                    funcionario.Nome = name;
                    gestor.ApplicationUser = user;
                    gestor.Empresa = empresa;
                    gestor.EmpresaId = empresa.Id;
                    _context.Add(gestor);
                    _context.Add(funcionario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(gestor);
        }
    }
}
