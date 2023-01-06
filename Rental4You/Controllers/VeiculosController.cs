﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

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
        [Authorize(Roles = "Gestor, Funcionario")]
        public async Task<IActionResult> Index(string? Estado = "Todos", int? CategoriaId = 0)
        {
            var categoriaTantofaz = new Categoria();
            categoriaTantofaz.Id = 0;
            categoriaTantofaz.Nome = "Todas";
            var categoraiasSelect = new List<Categoria>();
            categoraiasSelect.Add(categoriaTantofaz);
            categoraiasSelect.AddRange(_context.Categorias);
            ViewData["CategoriaId"] = new SelectList(categoraiasSelect, "Id", "Nome");
            var estados = new List<String>();
            estados.Add("Todos");
            estados.Add("Disponivel");
            estados.Add("Indisponivel");
            ViewData["Estados"] = new SelectList(estados);

            var empresa = getEmpresa();
            if (empresa == null)
                return NotFound();
            var veiculos = new List<Veiculo>();
            if (CategoriaId != 0)
            {
                if(Estado == null || Estado.Equals("Todos"))
                    veiculos = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).Where(v=> v.CategoriaId == CategoriaId && v.EmpresaId == empresa.Id).ToListAsync();
                else
                {
                    var estado = Estado.Equals("Disponivel") ? true : false;
                    veiculos = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).Where(v => v.Disponivel == estado && v.CategoriaId == CategoriaId && v.EmpresaId == empresa.Id).ToListAsync();
                }
            }
            else
            {
                if(Estado == null || Estado.Equals("Todos"))
                    veiculos = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).Where(v => v.EmpresaId == empresa.Id)
                        .ToListAsync();
                else
                {
                    var estado = Estado.Equals("Disponivel") ? true : false;
                    veiculos = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).Where(v => v.Disponivel == estado && v.EmpresaId == empresa.Id)
                        .ToListAsync();
                }
            }

            ViewBag.NomeEmpresa = empresa.Nome;
            return View(veiculos);
        }

       

        public bool VeiculoDisponivel(DateTime dataEntrega, DateTime dataLevantamento, int id)
        {

            var reservas = _context.Reservas.Where(v => v.VeiculoId == id).ToList();

            foreach (var reserva in reservas)
            {
                if ((dataLevantamento >= reserva.DataInicio && dataLevantamento < reserva.DataFim) ||
                    (dataEntrega > reserva.DataInicio && dataEntrega <= reserva.DataFim))
                {
                    if(reserva.Estado == false && reserva.Confirmado == false)
                    {
                        return false;
                    }
                }
                if(dataLevantamento < reserva.DataInicio && dataEntrega > reserva.DataFim)
                {
                    if (reserva.Estado == false && reserva.Confirmado == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        [Authorize]
        public async Task<IActionResult> Search(string? Localizacao, int? EmpresaId, int? CategoriaId, DateTime? DataLevantamento, DateTime? DataEntrega, string? sortBy)
        {
            var categoriaTantofaz = new Categoria();
            categoriaTantofaz.Id = 0;
            categoriaTantofaz.Nome = "Todas";
            var categoraiasSelect = new List<Categoria>();
            categoraiasSelect.Add(categoriaTantofaz);
            categoraiasSelect.AddRange(_context.Categorias);
            ViewData["CategoriaId"] = new SelectList(categoraiasSelect, "Id", "Nome");

            var tantoFaz = new Empresa();
            tantoFaz.Id = 0;
            tantoFaz.Nome = "Todas";
            var empresasSelect = new List<Empresa>();
            empresasSelect.Add(tantoFaz);
            empresasSelect.AddRange(_context.Empresas);
            ViewData["EmpresaId"] = new SelectList(empresasSelect, "Id", "Nome");

            var veiculos = _context.Veiculos.ToList();
            var localizacoes = new List<string>();
            localizacoes.Add("Todas");
            foreach (var v in veiculos)
            {
                if (!localizacoes.Contains(v.Localizacao))
                {
                    localizacoes.Add(v.Localizacao);
                }
            }
            ViewData["Localizacoes"] = new SelectList(localizacoes, "Localizacao");

            var pesquisaVM = new PesquisaVeiculosViewModel();

            if (string.IsNullOrWhiteSpace(Localizacao) || DataEntrega == null || DataLevantamento == null ||
                CategoriaId == null)
                pesquisaVM.ListaVeiculos = new List<Veiculo>() ;
            else
            {
                var lista = new List<Veiculo>();
                if (EmpresaId == null || EmpresaId == 0)
                {
                   if(CategoriaId == null || CategoriaId == 0 || Localizacao == "Todas") {
                        lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                       Where(c => c.Localizacao.Contains(Localizacao) &&
                       c.Disponivel == true && c.Empresa.EstadoSubscricao == true
                            ).ToListAsync();
                   } else
                   {
                        lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                            Where(c => c.Localizacao.Contains(Localizacao) && c.CategoriaId == CategoriaId &&
                            c.Disponivel == true && c.Empresa.EstadoSubscricao == true
                                ).ToListAsync();
                   }
                   
                }
                else
                {
                    if (CategoriaId == null || CategoriaId == 0 || Localizacao == "Todas")
                    {
                        lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                        Where(c => c.Localizacao.Contains(Localizacao) &&
                        c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                             ).ToListAsync();
                    }
                    else
                    {
                        lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                    Where(c => c.Localizacao.Contains(Localizacao) && c.CategoriaId == CategoriaId &&
                    c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                         ).ToListAsync();
                    }
                    
                }
                
                pesquisaVM.ListaVeiculos = new List<Veiculo>();
                foreach (var veiculo in lista)
                {
                    if (VeiculoDisponivel(pesquisaVM.DataEntrega, pesquisaVM.DataLevantamento, veiculo.Id) == true)
                        pesquisaVM.ListaVeiculos.Add(veiculo);
                }
            }
            if (!string.IsNullOrEmpty(sortBy)) {
                var newList = pesquisaVM.ListaVeiculos;
                switch (sortBy)
                {
                    case "Pmais":
                        newList = pesquisaVM.ListaVeiculos.OrderByDescending(v => v.Custo).ToList();
                        break;
                    case "Pmenos":
                        newList = pesquisaVM.ListaVeiculos.OrderBy(v => v.Custo).ToList();
                        break;
                    case "Rmais":
                        newList = pesquisaVM.ListaVeiculos.OrderByDescending(v => v.Empresa.Avalicao).ToList();
                        break;
                    case "Rmenos":
                        newList = pesquisaVM.ListaVeiculos.OrderBy(v => v.Empresa.Avalicao).ToList();
                        break;
                }
                pesquisaVM.ListaVeiculos = newList;
            }
            pesquisaVM.NumResultados = pesquisaVM.ListaVeiculos.Count();
            return View(pesquisaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(
            [Bind("Localizacao,CategoriaId,DataLevantamento,DataEntrega")]
            PesquisaVeiculosViewModel pesquisaVeiculo, string? sortBy, int? EmpresaId
            )
        {
            var categoriaTantofaz = new Categoria();
            categoriaTantofaz.Id = 0;
            categoriaTantofaz.Nome = "Todas";
            var categoraiasSelect = new List<Categoria>();
            categoraiasSelect.Add(categoriaTantofaz);
            categoraiasSelect.AddRange(_context.Categorias);
            ViewData["CategoriaId"] = new SelectList(categoraiasSelect, "Id", "Nome");

            var tantoFaz = new Empresa();
            tantoFaz.Id = 0;
            tantoFaz.Nome = "Todas";
            var empresasSelect = new List<Empresa>();
            empresasSelect.Add(tantoFaz);
            empresasSelect.AddRange(_context.Empresas);
            ViewData["EmpresaId"] = new SelectList(empresasSelect, "Id", "Nome");

            var veiculos = _context.Veiculos.ToList();
            var localizacoes = new List<string>();
            localizacoes.Add("Todas");
            foreach (var v in veiculos)
            {
                if (!localizacoes.Contains(v.Localizacao))
                {
                    localizacoes.Add(v.Localizacao);
                }
            }
            ViewData["Localizacoes"] = new SelectList(localizacoes, "Localizacao");
            if (pesquisaVeiculo.DataEntrega == pesquisaVeiculo.DataLevantamento)
            {
                pesquisaVeiculo.ListaVeiculos = new List<Veiculo>();
                return View(pesquisaVeiculo);
            }
            ModelState.Remove(nameof(PesquisaVeiculosViewModel.ListaVeiculos));
            ModelState.Remove(nameof(PesquisaVeiculosViewModel.NumResultados));
            if (string.IsNullOrEmpty(pesquisaVeiculo.Localizacao) || !ModelState.IsValid)
            {
                pesquisaVeiculo.ListaVeiculos = new List<Veiculo>();
            }
            else
            {
                var lista = new List<Veiculo>();
                if (EmpresaId == null || EmpresaId == 0)
                {
                    if (pesquisaVeiculo.CategoriaId == 0)
                    {   
                        if(pesquisaVeiculo.Localizacao == "Todas")
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa)
                                .Where(c => c.Disponivel == true && c.Empresa.EstadoSubscricao == true).ToListAsync();
                        } else
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa)
                                .Where(c => c.Localizacao.Contains(pesquisaVeiculo.Localizacao) && c.Disponivel == true && c.Empresa.EstadoSubscricao == true).ToListAsync();
                        }
                        
                        
                    }
                    else
                    {
                        if (pesquisaVeiculo.Localizacao == "Todas")
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                            Where(c => c.CategoriaId == pesquisaVeiculo.CategoriaId &&
                            c.Disponivel == true && c.Empresa.EstadoSubscricao == true
                                ).ToListAsync();
                        } 
                        else
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                            Where(c => c.Localizacao.Contains(pesquisaVeiculo.Localizacao) && c.CategoriaId == pesquisaVeiculo.CategoriaId &&
                            c.Disponivel == true && c.Empresa.EstadoSubscricao == true
                                ).ToListAsync();
                        }
                            
                    }

                }
                else
                {
                    if (pesquisaVeiculo.CategoriaId == 0)
                    {
                        if (pesquisaVeiculo.Localizacao == "Todas")
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                                Where(c => c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                                     ).ToListAsync();
                        } 
                        else
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                                                            Where(c => c.Localizacao.Contains(pesquisaVeiculo.Localizacao) &&
                                                            c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                                                                 ).ToListAsync();
                        }
                    }
                    else
                    {
                        if (pesquisaVeiculo.Localizacao == "Todas")
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                                        Where(c => c.CategoriaId == pesquisaVeiculo.CategoriaId &&
                                        c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                                             ).ToListAsync();
                        }
                        else
                        {
                            lista = await _context.Veiculos.Include(v => v.Categoria).Include(v => v.Empresa).
                                                Where(c => c.Localizacao.Contains(pesquisaVeiculo.Localizacao) && c.CategoriaId == pesquisaVeiculo.CategoriaId &&
                                                c.Disponivel == true && c.Empresa.EstadoSubscricao == true && c.EmpresaId == EmpresaId
                                                     ).ToListAsync();
                        }
                            
                    }

                }

                pesquisaVeiculo.ListaVeiculos = new List<Veiculo>();
                foreach(var veiculo in lista)
                {
                    if(VeiculoDisponivel(pesquisaVeiculo.DataEntrega, pesquisaVeiculo.DataLevantamento, veiculo.Id) == true)
                        pesquisaVeiculo.ListaVeiculos.Add(veiculo);
                }
                if (!string.IsNullOrEmpty(sortBy))
                {
                    var newList = pesquisaVeiculo.ListaVeiculos;
                    switch (sortBy)
                    {
                        case "Pmais":
                            newList = pesquisaVeiculo.ListaVeiculos.OrderByDescending(v => v.Custo).ToList();
                            break;
                        case "Pmenos":
                            newList = pesquisaVeiculo.ListaVeiculos.OrderBy(v => v.Custo).ToList();
                            break;
                        case "Rmais":
                            newList = pesquisaVeiculo.ListaVeiculos.OrderByDescending(v => v.Empresa.Avalicao).ToList();
                            break;
                        case "Rmenos":
                            newList = pesquisaVeiculo.ListaVeiculos.OrderBy(v => v.Empresa.Avalicao).ToList();
                            break;
                    }
                    pesquisaVeiculo.ListaVeiculos = newList;
                }
            }
            pesquisaVeiculo.NumResultados = pesquisaVeiculo.ListaVeiculos.Count();

            

            return View(pesquisaVeiculo);
        }



        // GET: Veiculos/Create
        [Authorize(Roles = "Gestor, Funcionario")]
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
        [Authorize(Roles = "Gestor, Funcionario")]
        public async Task<IActionResult> Create([Bind("Id,Marca,Modelo,CategoriaId,Localizacao,NumeroLugares,Caixa,Disponivel,Custo,Danos,Kilometros,EmpresaId")] Veiculo veiculo)
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
        [Authorize(Roles = "Gestor, Funcionario")]
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
        [Authorize(Roles = "Gestor, Funcionario")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,CategoriaId,Localizacao,NumeroLugares,Caixa,Disponivel,Custo,Danos,Kilometros,EmpresaId")] Veiculo veiculo)
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
        [Authorize(Roles = "Gestor, Funcionario")]
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
        [Authorize(Roles = "Gestor, Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculos'  is null.");
            }
            var veiculo = await _context.Veiculos.FindAsync(id);
            var reservas = _context.Reservas.Where(r => r.VeiculoId == id).ToList();
            foreach(var reserva in reservas)
            {
                if(reserva.VeiculoId == id)
                {
                    if (reserva.Confirmado == false)
                    {
                        ViewData["AlertMessage"] = "Veículo encontra-se reservado";
                        return View(veiculo);
                    }
                    else if(reserva.FuncionarioRecebeId == null)
                    {
                        ViewData["AlertMessage"] = "Veículo encontra-se reservado";
                        return View(veiculo);
                    }
                }
            }
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
