using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FuncionariosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Funcionarios
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var gestor = _context.Gestores.Include(g => g.ApplicationUser).Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            if (gestor == null)
                return (NotFound());

            var applicationDbContext = _context.Funcionarios.Include(f => f.Empresa).Include(f=>f.ApplicationUser).Where(f => _userManager.GetUsersInRoleAsync("Funcionario").Result.Contains(f.ApplicationUser));
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Funcionarios/Create
        [Authorize(Roles = "Gestor")]
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

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Funcionario funcionario)
        {
            var userId = _userManager.GetUserId(User);
            var gestorTemp = _context.Gestores.Include(g => g.ApplicationUser).Include(g => g.Empresa)
                .Where(g => g.ApplicationUser.Id == userId).FirstOrDefault();
            if (gestorTemp == null)
                return NotFound();
            var empresa = _context.Empresas.Where(e => e.Id == gestorTemp.EmpresaId).FirstOrDefault();
            if (empresa == null)
                return NotFound();
            var name = funcionario.Nome.Replace(" ", "");
            ApplicationUser user = new ApplicationUser();
            user.EmailConfirmed = true;
            user.Ativo = true;
            user.UserName = name + "@" + empresa.Nome + ".com";
            user.Email = user.UserName;
            var result = await _userManager.CreateAsync(user, "Is3C..00");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Funcionario");
                ModelState.Remove(nameof(Funcionario.Empresa));
                ModelState.Remove(nameof(Gestor.ApplicationUser));
                if (ModelState.IsValid)
                {
                    funcionario.ApplicationUser = user;
                    funcionario.Empresa = empresa;
                    funcionario.EmpresaId = empresa.Id;
                    _context.Add(funcionario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(funcionario);
        }

        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Edit(int userId)
        {
            var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser).Where(f => f.Id == userId).FirstOrDefault();
            if (funcionario == null)
                return NotFound();

            var user = funcionario.ApplicationUser;
            if (user == null)
            {
                return NotFound();
            }
            if(!_userManager.GetUsersInRoleAsync("Funcionario").Result.Contains(user))
            {
                return RedirectToAction(nameof(Index));
            }
            var userModel = new ApplicationUserViewModel();
            userModel.UserId = user.Id;
            userModel.UserName = user.UserName;
            userModel.Roles = await GetUserRoles(user);
            userModel.PrimeiroNome = user.PrimeiroNome ?? "";
            userModel.UltimoNome = user.UltimoNome ?? "";
            userModel.Ativo = user.Ativo;

            return View(userModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [HttpPost]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Edit(ApplicationUserViewModel model, int userId)
        {
            var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser).Where(f => f.Id == userId).FirstOrDefault();
            if (funcionario == null)
                return NotFound();

            var user = funcionario.ApplicationUser;
            if (user == null)
            {
                return NotFound();
            }
            user.Ativo = model.Ativo;
            user.EmailConfirmed = model.Ativo;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot update user");
                return View(user);
            }

            return RedirectToAction("Index");
        }
    }
}
