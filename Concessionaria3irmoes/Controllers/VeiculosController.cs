using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Concessionaria3irmoes.Data;
using Concessionaria3irmoes.Models;

namespace Concessionaria3irmoes.Controllers
{
    /// Controlador responsável pelo catálogo de veículos.
    /// Gerencia a exibição para clientes e a administração (CRUD) para funcionários.
    [Authorize]// Exige login para acessar qualquer parte, mas as Roles definem o nível de acesso abaixo.
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Veiculos
        /// Exibe a lista de veículos.
        /// LÓGICA DE NEGÓCIO: Se for Admin, vê tudo. Se for Cliente, vê apenas os não vendidos.
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))// Admin: Visualiza todo o histórico, inclusive carros já vendidos (controle de estoque)
            {
                return View(await _context.Veiculos.ToListAsync());
            }
            else
            {
                // Cliente: Visualiza apenas carros disponíveis para compra (!v.Vendido)
                return View(await _context.Veiculos.Where(v => !v.Vendido).ToListAsync());
            }
        }

        // GET: Veiculos/Details/5
        /// Exibe os detalhes técnicos de um veículo específico
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculoModel == null)
            {
                return NotFound();
            }

            return View(veiculoModel);
        }

        // GET: Veiculos/Create
        // SEGURANÇA: Apenas Administradores podem acessar a tela de cadastro.
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veiculos/Create
        /// Recebe os dados do formulário para salvar um novo veículo.
        /// [ValidateAntiForgeryToken]: Impede ataques CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Modelo,Marca,Preco,Motor,Potencia,Quilometragem,Ano,Vendido")] VeiculoModel veiculoModel)
        {
            if (ModelState.IsValid)
            {
                veiculoModel.Vendido = false;
                _context.Add(veiculoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoModel);
        }

        // GET: Veiculos/Edit/5
        // SEGURANÇA: Clientes não podem ver a tela de edição.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculos.FindAsync(id);
            if (veiculoModel == null)
            {
                return NotFound();
            }
            return View(veiculoModel);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Modelo,Marca,Preco,Motor,Potencia,Quilometragem,Ano,Vendido")] VeiculoModel veiculoModel)
        {
            if (id != veiculoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoModelExists(veiculoModel.Id))
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
            return View(veiculoModel);
        }

        // GET: Veiculos/Delete/5
        // SEGURANÇA: Apenas Admin pode acessar a tela de confirmação de exclusão.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculoModel == null)
            {
                return NotFound();
            }

            return View(veiculoModel);
        }

        // POST: Veiculos/Delete/5
        // Exclusão real do banco de dados.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veiculoModel = await _context.Veiculos.FindAsync(id);
            if (veiculoModel != null)
            {
                _context.Veiculos.Remove(veiculoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoModelExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}
