using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using System.Diagnostics;

namespace Rental4You.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {   
            // Create a "All" category with an Id of 0
            var todasCategorias = new Categoria { Id = 0, Nome = "Todas" };

            // Get the list of categories from the database
            var categorias = _context.Categorias.ToList();

            // Insert the "All" category at the beginning of the list
            categorias.Insert(0, todasCategorias);
            ViewData["CategoriaId"] = new SelectList(categorias, "Id", "Nome");

            // Get a list of all vehicles from the database
            var veiculos = _context.Veiculos.ToList();
            // Create a list to store the unique localizations
            var localizacoes = new List<string>();

            localizacoes.Add("Todas");

            // Loop through each vehicle in the list
            foreach (var v in veiculos)
            {
                // If the localization is not already in the list, add it
                if (!localizacoes.Contains(v.Localizacao))
                {
                    localizacoes.Add(v.Localizacao);
                }
            }

            // Set the ViewBag property to the list of unique localizations
            ViewData["Localizacoes"] = new SelectList(localizacoes,"Localizacao");
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("home/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}