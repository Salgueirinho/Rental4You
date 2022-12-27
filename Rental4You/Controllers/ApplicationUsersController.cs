using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace PWEB_AulasP_2223.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
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
        public async Task<IActionResult> Details(ApplicationUserViewModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Ativo = model.Ativo;
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
