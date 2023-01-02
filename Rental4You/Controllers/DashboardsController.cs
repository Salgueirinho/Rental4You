using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using System.Data;
using System.Drawing.Printing;

namespace Rental4You.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminBoard()
        {
            return View();
        }
        public IActionResult GestorBoard()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDadosReservasDiarias()
        {
            //dados de exemplo
            List<object> dados = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Reservas", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var reservas = _context.Reservas.Where(r => r.DataConfirmada > DateTime.Now.AddDays(-30)).ToList();
            DateTime dateTime = DateTime.Now.AddDays(-30);
            while(dateTime <= DateTime.Now) {
                DataRow dr = dt.NewRow();
                dr["Reservas"] = dateTime.ToShortDateString();
                int quant = 0;
                foreach( var r in reservas)
                {
                    if (r.DataConfirmada.Date == dateTime.Date)
                        quant++;
                }
                dateTime = dateTime.AddDays(1);
                dr["Quantidade"] = quant;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDadosReservasMensais()
        {
            //dados de exemplo
            List<object> dados = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Reservas", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var reservas = _context.Reservas.Where(r => r.DataConfirmada > DateTime.Now.AddMonths(-12)).ToList();
            DateTime dateTime = DateTime.Now.AddMonths(-12);
            while (dateTime <= DateTime.Now)
            {
                DataRow dr = dt.NewRow();
                dr["Reservas"] = dateTime.ToString("MMM");
                int quant = 0;
                foreach (var r in reservas)
                {
                    if (r.DataConfirmada.Month == dateTime.Month && r.DataConfirmada.Year == dateTime.Year)
                        quant++;
                }
                dateTime = dateTime.AddMonths(1);
                dr["Quantidade"] = quant;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetClientesMensais()
        {
            //dados de exemplo
            List<object> dados = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Clientes", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var clientes = _context.Clientes.Include(c => c.ApplicationUser).Where(c => c.ApplicationUser.DataRegisto > DateTime.Now.AddMonths(-12)).ToList();
            DateTime dateTime = DateTime.Now.AddMonths(-12);
            while (dateTime <= DateTime.Now)
            {
                DataRow dr = dt.NewRow();
                dr["Clientes"] = dateTime.ToString("MMM");
                int quant = 0;
                foreach (var c in clientes)
                {
                    if (c.ApplicationUser.DataRegisto.Month == dateTime.Month && c.ApplicationUser.DataRegisto.Year == dateTime.Year)
                        quant++;
                }
                dateTime = dateTime.AddMonths(1);
                dr["Quantidade"] = quant;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);

        }

        [HttpPost]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> GetFaturacaoSemanal()
        {
            //dados de exemplo
            List<object> dados = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Faturação", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var reservas = _context.Reservas.Where(r => r.DataConfirmada > DateTime.Now.AddDays(-7)).ToList();
            DateTime dateTime = DateTime.Now.AddDays(-7);
            while (dateTime <= DateTime.Now)
            {
                DataRow dr = dt.NewRow();
                dr["Faturação"] = dateTime.ToShortDateString();
                decimal faturacao = 0;
                foreach (var r in reservas)
                {
                    if (r.DataConfirmada.Date == dateTime.Date)
                        faturacao += r.Valor;
                }
                dateTime = dateTime.AddDays(1);
                dr["Quantidade"] = faturacao;
                faturacao = 0;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);

        }

        [HttpPost]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> GetFaturacaoMensal()
        {
            //dados de exemplo
            List<object> dados = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Faturação", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));

            var reservas = _context.Reservas.Where(r => r.DataConfirmada > DateTime.Now.AddDays(-30)).ToList();
            DateTime dateTime = DateTime.Now.AddDays(-30);
            while (dateTime <= DateTime.Now)
            {
                DataRow dr = dt.NewRow();
                dr["Faturação"] = dateTime.ToShortDateString();
                decimal faturacao = 0;
                foreach (var r in reservas)
                {
                    if (r.DataConfirmada.Date == dateTime.Date)
                        faturacao += r.Valor;
                }
                dateTime = dateTime.AddDays(1);
                dr["Quantidade"] = faturacao;
                faturacao = 0;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }
            return Json(dados);

        }
    }
}
