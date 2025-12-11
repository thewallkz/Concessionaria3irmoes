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
    /// Controlador responsável pela gestão de Clientes (CRUD).
    /// SEGURANÇA: Esta classe inteira é restrita ao perfil 'Admin' para proteger dados sensíveis (LGPD).
    /// Clientes comuns não podem acessar, visualizar ou editar dados de outros clientes.
    [Authorize(Roles = "Admin")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Construtor: Utiliza Injeção de Dependência para acessar o Banco de Dados
        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        // Exibe a lista completa de clientes cadastrados.
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        // Exibe os detalhes completos de um cliente específico (CPF, Endereço, etc).
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteModel = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clienteModel == null)
            {
                return NotFound();
            }

            return View(clienteModel);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // Recebe os dados do formulário e salva no banco de dados.
        // [ValidateAntiForgeryToken]: Protege contra ataques de falsificação de solicitação (CSRF).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,CPF,Endereco,Email,Telefone")] ClienteModel clienteModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clienteModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clienteModel);
        }

        // GET: Clientes/Edit/5
        // Renderiza o formulário preenchido com os dados do cliente para edição.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteModel = await _context.Clientes.FindAsync(id);
            if (clienteModel == null)
            {
                return NotFound();
            }
            return View(clienteModel);
        }

       // POST: Clientes/Edit/5
        // Processa a atualização dos dados do cliente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CPF,Endereco,Email,Telefone")] ClienteModel clienteModel)
        {
            if (id != clienteModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clienteModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteModelExists(clienteModel.Id))
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
            return View(clienteModel);
        }

        // GET: Clientes/Delete/5
        // Exibe uma página de confirmação antes de excluir o cliente.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteModel = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clienteModel == null)
            {
                return NotFound();
            }

            return View(clienteModel);
        }

        // POST: Clientes/Delete/5
        // Ação definitiva que remove o registro do banco de dados.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clienteModel = await _context.Clientes.FindAsync(id);
            if (clienteModel != null)
            {
                _context.Clientes.Remove(clienteModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Método auxiliar para verificar se um cliente existe pelo ID.
        private bool ClienteModelExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
