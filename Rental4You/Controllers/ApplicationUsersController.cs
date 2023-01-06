using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace PWEB_AulasP_2223.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public ApplicationUsersController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(ApplicationUser user)
        {
            var users = await _userManager.Users.ToListAsync();
            
            var usersViewModel = new List<ApplicationUserViewModel>();

            foreach (var u in users)
            {
                var userModel = new ApplicationUserViewModel();
                userModel.UserId = u.Id;
                userModel.UserName = u.UserName;
                userModel.Roles = await GetUserRoles(u);
                userModel.PrimeiroNome = u.PrimeiroNome?? "";
                userModel.UltimoNome = u.UltimoNome?? "";
                userModel.Ativo = u.Ativo;
                usersViewModel.Add(userModel);
            }

            return View(usersViewModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userModel = new ApplicationUserViewModel();
            userModel.UserId = user.Id;
            userModel.UserName = user.UserName;
            userModel.Roles = await GetUserRoles(user);
            userModel.PrimeiroNome = user.PrimeiroNome?? "";
            userModel.UltimoNome = user.UltimoNome?? "";
            userModel.Ativo = user.Ativo;

            return View(userModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(ApplicationUserViewModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Ativo = model.Ativo;
            user.PrimeiroNome = model.PrimeiroNome;
            user.UltimoNome = model.UltimoNome;
            user.EmailConfirmed = user.Ativo;
            var result = await _userManager.UpdateAsync(user);

            if (User.IsInRole("Gestor"))
            {
                var gestor = _context.Gestores.Include(g => g.ApplicationUser).Where(g => g.ApplicationUser.Id == user.Id)
                    .FirstOrDefault();
                if (gestor == null)
                    return NotFound();
                gestor.Nome = (user.PrimeiroNome + user.UltimoNome).Trim().Replace(" ", "");

                var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser).Where(f => f.ApplicationUser.Id == user.Id)
                    .FirstOrDefault();
                if (funcionario == null)
                    return NotFound();
                funcionario.Nome = (user.PrimeiroNome + user.UltimoNome).Trim().Replace(" ", "");
                _context.Update(funcionario);
                _context.Update(gestor);
                _context.SaveChanges();
            }

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser).Where(f => f.ApplicationUser.Id == user.Id)
                    .FirstOrDefault();
                if (funcionario == null)
                    return NotFound();
                funcionario.Nome = (user.PrimeiroNome + user.UltimoNome).Trim().Replace(" ", "");
                _context.Update(funcionario);
                _context.SaveChanges();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot update user");
                return View(user);
            }

            return RedirectToAction("Index");
        }
    }
}
