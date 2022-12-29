using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Rental4You.Data;
using Rental4You.Models;

namespace Rental4You.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var empresa = getEmpresa();
            if (empresa == null)
                return NotFound();
            ViewBag.NomeEmpresa = empresa.Nome;
            var applicationDbContext = _context.Reservas.Include(r => r.Cliente).Include(r => r.FuncionarioEntrega)
                .Include(r => r.FuncionarioRecebe).Include(r => r.Veiculo).Include(r=>r.Cliente.ApplicationUser)
                .Where(r => r.Veiculo.EmpresaId == empresa.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ReservasCliente()
        {
            var userId = _userManager.GetUserId(User);
            var cliente = _context.Clientes.Include(c => c.ApplicationUser).Where(c => c.ApplicationUser.Id == userId)
                .FirstOrDefault();
            if (cliente == null)
                return NotFound();
            var applicationDbContext = _context.Reservas.Include(r => r.Cliente).Include(r => r.FuncionarioEntrega)
                .Include(r => r.FuncionarioRecebe).Include(r => r.Veiculo).Include(r => r.Cliente.ApplicationUser)
                .Where(r => r.ClienteId == cliente.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        //Get: Confirmar
        public async Task<IActionResult> ConfirmarReserva(int id)
        {
            var reserva = _context.Reservas.Where(r => r.Id == id).FirstOrDefault();
            if (reserva == null)
                return NotFound();
            if (reserva.Confirmado == false)
            {
                reserva.Confirmado = true;
                reserva.Estado = true;
            }
            _context.Update(reserva);
            await _context.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }

        //Get: Rejeitar
        public async Task<IActionResult> RejeitarReserva(int id)
        {
            Console.WriteLine("Rejeitando reserva");
            var reserva = _context.Reservas.Where(r => r.Id == id).FirstOrDefault();
            if (reserva == null)
                return NotFound();
            if (reserva.Confirmado == false)
            {
                if (reserva.Confirmado == false)
                {
                    reserva.Confirmado = true;
                    reserva.Estado = false;
                }
            }
            _context.Update(reserva);
            await _context.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.FuncionarioEntrega)
                .Include(r => r.FuncionarioRecebe)
                .Include(r => r.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var cliente = _context.Clientes.Include(c => c.ApplicationUser).Where(c => c.ApplicationUser.Id == userId).FirstOrDefault();
            if (cliente == null || cliente.Id != reserva.ClienteId)
                return NotFound();

            if (Directory.Exists("wwwroot\\DanosImagens\\" + id))
            {
                string[] filenamesList = Directory.GetFiles("wwwroot\\DanosImagens\\" + id);
                List<string> filenames = new List<string>();
                foreach (string filename in filenamesList)
                {
                    filenames.Add(filename.Replace("wwwroot", ""));
                }
                ViewBag.images = filenames;
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create(int? id, string? data_levantamento, string? data_entrega)
        {
            Console.WriteLine(data_levantamento);
            Console.WriteLine(data_entrega);
            if (id == null || data_levantamento == null || data_entrega == null)
                return NotFound();
            var dataEntrega = DateTime.Parse(data_entrega);
            var dataLevantamento = DateTime.Parse(data_levantamento);

            var veiculo = _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).Where(v => v.Id == id).FirstOrDefault();
            if (veiculo == null)
                return NotFound();
            var dias = (dataEntrega - dataLevantamento).TotalDays;
            if (dias <= 0)
                return NotFound();
            var userId = _userManager.GetUserId(User);
            var cliente = _context.Clientes.Include(c => c.ApplicationUser)
                .Where(c => c.ApplicationUser.Id == userId).FirstOrDefault();
            if (cliente == null)
                return NotFound();

            var preco = (decimal)dias * veiculo.Custo;
            var reserva = new Reserva();
            reserva.Estado = false;
            reserva.VeiculoId = veiculo.Id;
            reserva.Veiculo = veiculo;
            reserva.ClienteId = cliente.Id;
            reserva.Cliente = cliente;
            reserva.Confirmado = false;
            reserva.DataInicio = dataLevantamento;
            reserva.DataFim = dataEntrega;
            reserva.KilometrosInicio = veiculo.Kilometros;

            ViewBag.PrecoReserva = preco;
            return View(reserva);
        }

        // Get: Confirmar

        public async Task<IActionResult> Confirmar()
        {
            return RedirectToAction(nameof(ReservasCliente));
        }


        // POST: Reservas/Confirmar
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar([Bind("Id,VeiculoId,KilometrosInicio,KilometrosFim,Estado,DataInicio,DataFim,ClienteId,FuncionarioEntregaId,FuncionarioRecebeId,ObservacoesInicio,ObservacoesFim")] Reserva reserva)
        {
            reserva.Confirmado = false;
            reserva.Estado = false;
            reserva.DataConfirmada = DateTime.Now;
            if (VeiculoDisponivel(reserva.DataFim, reserva.DataInicio, reserva.VeiculoId) == false)
            {
                ModelState.AddModelError("Veículo Indisponivel", "Parece que o veículo já se encontra alugado :(");
                return View(reserva);
            } 
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ReservasCliente));
            }
            return View(reserva);
        }
        public bool VeiculoDisponivel(DateTime dataEntrega, DateTime dataLevantamento, int id)
        {
            var reservas = _context.Reservas.Where(v => v.VeiculoId == id).ToList();
            foreach (var reserva in reservas)
            {
                if ((dataLevantamento >= reserva.DataInicio && dataLevantamento < reserva.DataFim) ||
                    (dataEntrega > reserva.DataInicio && dataEntrega <= reserva.DataFim))
                {
                    if (reserva.Estado == false && reserva.Confirmado == false)
                    {
                        return false;
                    }
                }
                if (dataLevantamento < reserva.DataInicio && dataEntrega > reserva.DataFim)
                {
                    if (reserva.Estado == false && reserva.Confirmado == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> EntregarVeiculo(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = _context.Reservas.Include(r => r.Cliente).Include(r=>r.Cliente.ApplicationUser)
                .Include(r => r.Veiculo).Where(r => r.Id == id).FirstOrDefault();
            if (reserva == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser)
                .Where(f => f.ApplicationUser.Id == userId).FirstOrDefault();
            if(funcionario == null)
            {
                return NotFound();
            }
            reserva.FuncionarioEntrega = funcionario;
            reserva.FuncionarioEntregaId = funcionario.Id;
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> ReceberVeiculo(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = _context.Reservas.Include(r => r.Cliente).Include(r => r.Cliente.ApplicationUser)
                .Include(r => r.Veiculo).Where(r => r.Id == id).FirstOrDefault();
            if (reserva == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var funcionario = _context.Funcionarios.Include(f => f.ApplicationUser)
                .Where(f => f.ApplicationUser.Id == userId).FirstOrDefault();
            if (funcionario == null)
            {
                return NotFound();
            }
            reserva.FuncionarioRecebe = funcionario;
            reserva.FuncionarioRecebeId = funcionario.Id;
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VeiculoId,KilometrosInicio,KilometrosFim,Estado,DataInicio,DataFim,ClienteId,FuncionarioEntregaId,FuncionarioRecebeId,ObservacoesInicio,ObservacoesFim")] Reserva reserva,
            [FromForm] List<IFormFile> DanoImagem)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var veiculo = _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa)
                        .Where(v => v.Id == reserva.VeiculoId).FirstOrDefault();
                    if (veiculo == null)
                        return NotFound();
                    if (reserva.KilometrosFim < reserva.KilometrosInicio)
                        return NotFound();
                    veiculo.Kilometros = reserva.KilometrosFim?? veiculo.Kilometros;
                    _context.Update(veiculo);
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();

                    string path = Directory.GetCurrentDirectory();
                    Path.Combine(path, "wwwroot\\DanosImagens");

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string reservaPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\DanosImagens\\" + id.ToString());

                    if (!Directory.Exists(reservaPath))
                        Directory.CreateDirectory(reservaPath);

                    foreach (var formFile in DanoImagem)
                    {
                        if (formFile.Length > 0)
                        {
                            var filePath = Path.Combine(reservaPath, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));

                            while (System.IO.File.Exists(filePath))
                            {
                                filePath = Path.Combine(reservaPath, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                            }
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.FuncionarioEntrega)
                .Include(r => r.FuncionarioRecebe)
                .Include(r => r.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reserva'  is null.");
            }
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
          return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
