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
            var applicationDbContext = _context.Vendas.Include(v => v.Cliente).Include(v => v.Veiculo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vendas/Details/5
        // Mostra os detalhes de uma venda específica.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendaModel = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendaModel == null)
            {
                return NotFound();
            }

            return View(vendaModel);
        }

       
       // GET: Vendas/Create
public IActionResult Create()
{
    // 1. CARREGAR CLIENTES: Usamos o "Nome" para o texto visível.
    ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");

    
    // 2. CARREGAR VEÍCULOS: Criamos um texto descritivo mais completo para o usuário.
    var veiculosParaLista = _context.Veiculos
        .Select(v => new
        {
            // O valor real que será salvo no banco
            Id = v.Id, 
            
            // O texto formatado que o usuário verá: Marca Modelo (Ano) - R$ Preço
            DescricaoCompleta = v.Marca + " " + v.Modelo + " (" + v.Ano + ") - R$ " + v.Preco.ToString("N0")
        })
        .ToList();

    // Cria a SelectList para a View usando o texto formatado.
    ViewData["VeiculoId"] = new SelectList(veiculosParaLista, "Id", "DescricaoCompleta");

    // Envia um objeto VendaModel com a data atual para preencher o campo DataVenda na View
    var venda = new VendaModel { DataVenda = DateTime.Now };
    return View(venda);
}

        // POST: Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataVenda,ValorFinal,ClienteId,VeiculoId")] VendaModel vendaModel)
        {
            if (ModelState.IsValid)
            {
                // 1. Busca o veículo no banco
                var veiculo = await _context.Veiculos.FindAsync(vendaModel.VeiculoId);

                // 2. Verifica se ele existe e se já não foi vendido (segurança)
                if (veiculo == null)
                {
                    ModelState.AddModelError("", "Veículo não encontrado.");
                }
                else if (veiculo.Vendido)
                {
                    ModelState.AddModelError("", "Desculpe, este veículo já foi vendido por outro cliente agora mesmo!");
                }
                else
                {
                    // 3. TUDO CERTO: Marca como vendido
                    veiculo.Vendido = true;
                    _context.Update(veiculo); // Atualiza o carro

                    // 4. Salva a Venda
                    _context.Add(vendaModel);
                    await _context.SaveChangesAsync();
                    
                    // Sucesso: Volta para a Home ou Lista de Veículos
                    return RedirectToAction("Index", "Veiculos"); 
                }
            }
            // Se algo deu errado, recarrega as listas
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", vendaModel.ClienteId);
            
            // Recarrega a lista bonita de novo
             var veiculosParaLista = _context.Veiculos
                .Where(v => !v.Vendido)
                .Select(v => new { Id = v.Id, DescricaoCompleta = $"{v.Marca} {v.Modelo} - {v.Preco:C2}" })
                .ToList();
            ViewData["VeiculoId"] = new SelectList(veiculosParaLista, "Id", "DescricaoCompleta", vendaModel.VeiculoId);
            
            return View(vendaModel);
        }

        // GET: Vendas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendaModel = await _context.Vendas.FindAsync(id);
            if (vendaModel == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", vendaModel.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", vendaModel.VeiculoId);
            return View(vendaModel);
        }

        // POST: Vendas/Edit/5
        // Apenas Admin pode editar uma venda (ex: corrigir valor ou data).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataVenda,ValorFinal,ClienteId,VeiculoId")] VendaModel vendaModel)
        {
            if (id != vendaModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaModelExists(vendaModel.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", vendaModel.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", vendaModel.VeiculoId);
            return View(vendaModel);
        }

        // GET: Vendas/Delete/5
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendaModel = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendaModel == null)
            {
                return NotFound();
            }

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
