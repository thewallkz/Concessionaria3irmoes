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
    /// Controlador responsável pelo Gerenciamento de Vendas.
    /// Realiza a baixa no estoque de veículos e gera o histórico financeiro.
    [Authorize]// Exige que o usuário esteja logado para acessar qualquer função
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendas
        // Exibe o relatório de vendas realizadas.
        // SEGURANÇA: Restrito ao Admin (Clientes não devem ver quanto a loja faturou).
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var vendas = _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Veiculo);
            return View(await vendas.ToListAsync());
        }

        // GET: Vendas/Details/5
        // Mostra os detalhes de uma venda específica.
        [Authorize(Roles = "Admin")]
       public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vendaModel = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vendaModel == null) return NotFound();

            return View(vendaModel);
        }

       
       // GET: Vendas/Create
        public async Task<IActionResult> Create(int? veiculoId)
        {
            if (veiculoId == null)
            {
                return BadRequest("É necessário escolher um veículo para iniciar a venda.");
            }

            var veiculo = await _context.Veiculos.FindAsync(veiculoId);

            // Validação: Carro existe? Já foi vendido?
            if (veiculo == null || veiculo.Vendido)
            {
                return BadRequest("Este veículo não está disponível ou já foi vendido.");
            }

            // Prepara a venda com os valores iniciais
            var venda = new VendaModel
            {
                VeiculoId = veiculo.Id,
                ValorOriginal = veiculo.Preco,
                ValorFinal = veiculo.Preco, // Começa igual ao original
                Desconto = 0,
                DataVenda = DateTime.Now
            };

            // ViewBags para exibir dados na tela
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["VeiculoNome"] = $"{veiculo.Marca} {veiculo.Modelo} ({veiculo.Ano})"; 

            return View(venda);
        }

        // POST: Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaModel venda)
        {
            // Busca o veículo novamente para garantir segurança
            var veiculo = await _context.Veiculos.FindAsync(venda.VeiculoId);

            if (veiculo == null) return NotFound();

            // REGRA DE NEGÓCIO: Recalcular valores no Back-end
            venda.ValorOriginal = veiculo.Preco;
            venda.ValorFinal = venda.ValorOriginal - venda.Desconto;

            // Validação 1: Valor negativo
            if (venda.ValorFinal < 0)
            {
                ModelState.AddModelError("Desconto", "O desconto não pode ser maior que o valor do veículo.");
            }

            // Validação 2: Veículo já vendido (concorrência)
            if (veiculo.Vendido)
            {
                ModelState.AddModelError("", "Este veículo acabou de ser vendido por outro vendedor.");
            }

            if (ModelState.IsValid)
            {
                // 1. Salva a Venda
                _context.Add(venda);

                // 2. Baixa no Estoque
                veiculo.Vendido = true;
                _context.Update(veiculo);

                await _context.SaveChangesAsync();
                
                // Redireciona para a lista de veículos após vender
                return RedirectToAction("Index", "Veiculos");
            }

            // Se der erro, recarrega os dados para a tela não quebrar
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", venda.ClienteId);
            ViewData["VeiculoNome"] = $"{veiculo.Marca} {veiculo.Modelo}";
            return View(venda);
        }

        // GET: Vendas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vendaModel = await _context.Vendas.FindAsync(id);
            if (vendaModel == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", vendaModel.ClienteId);
            return View(vendaModel);
        }

        // POST: Vendas/Edit/5
        // Apenas Admin pode editar uma venda (ex: corrigir valor ou data).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, VendaModel vendaModel)
        {
            if (id != vendaModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaModelExists(vendaModel.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", vendaModel.ClienteId);
            return View(vendaModel);
        }

        // GET: Vendas/Delete/5
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vendaModel = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vendaModel == null) return NotFound();

            return View(vendaModel);
        }

        // POST: Vendas/Delete/5
        // Apenas Admin pode cancelar/excluir uma venda.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendaModel = await _context.Vendas.FindAsync(id);
            
            if (vendaModel != null)
            {
                // REGRA DE ESTORNO: Se apagar a venda, o carro volta para o estoque
                var veiculo = await _context.Veiculos.FindAsync(vendaModel.VeiculoId);
                if (veiculo != null)
                {
                    veiculo.Vendido = false; // Devolve para a loja
                    _context.Update(veiculo);
                }

                _context.Vendas.Remove(vendaModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VendaModelExists(int id)
        {
            return _context.Vendas.Any(e => e.Id == id);
        }
    }
}
