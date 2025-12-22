using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Concessionaria3irmoes.Data;
using Concessionaria3irmoes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Concessionaria3irmoes.Controllers
{
    /// Controlador responsável pelo catálogo de veículos.
    /// Gerencia a exibição para clientes e a administração (CRUD) para funcionários.
    [Authorize] // Exige login para acessar qualquer parte, mas as Roles definem o nível de acesso abaixo.
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VeiculosController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Veiculos
        /// Exibe a lista de veículos.
        /// LÓGICA DE NEGÓCIO: Se for Admin, vê tudo. Se for Cliente, vê apenas os não vendidos.
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                // ADICIONADO: .Include(v => v.Fotos)
                // Isso obriga o banco a trazer as fotos junto com o carro
                return View(await _context.Veiculos.Include(v => v.Fotos).ToListAsync());
            }
            else
            {
                // ADICIONADO: .Include(v => v.Fotos) aqui também
                return View(
                    await _context
                        .Veiculos.Include(v => v.Fotos)
                        .Where(v => !v.Vendido)
                        .ToListAsync()
                );
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

            var veiculoModel = await _context
                .Veiculos.Include(v => v.Fotos)
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
        public async Task<IActionResult> Create(VeiculoModel veiculo)
        {
            if (ModelState.IsValid)
            {
                // Lógica das Fotos
                if (veiculo.FotosUpload != null && veiculo.FotosUpload.Count > 0)
                {
                    // Validação: Máximo 5 fotos
                    if (veiculo.FotosUpload.Count > 5)
                    {
                        ModelState.AddModelError("", "Você pode enviar no máximo 5 fotos.");
                        return View(veiculo);
                    }

                    string pastaDestino = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "imagens_veiculos"
                    );

                    // Cria a pasta se não existir
                    if (!Directory.Exists(pastaDestino))
                        Directory.CreateDirectory(pastaDestino);

                    foreach (var arquivo in veiculo.FotosUpload)
                    {
                        // Gera nome único para não substituir fotos iguais (Ex: guid_nome.jpg)
                        string nomeArquivo = Guid.NewGuid().ToString() + "_" + arquivo.FileName;
                        string caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

                        // Salva o arquivo na pasta
                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(stream);
                        }

                        // Adiciona na lista do objeto para salvar no banco
                        veiculo.Fotos.Add(new VeiculoFoto { CaminhoArquivo = nomeArquivo });
                    }
                }

                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veiculo);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Modelo,Marca,Preco,Motor,Potencia,Quilometragem,Ano,Vendido")]
                VeiculoModel veiculoFormulario
        )
        {
            if (id != veiculoFormulario.Id)
            {
                return NotFound();
            }

            // 1. Remove validações de campos que não estão neste formulário de edição
            // Isso impede que o ModelState falhe porque a lista de fotos veio vazia/nula
            ModelState.Remove("Fotos");
            ModelState.Remove("FotosUpload");

            if (ModelState.IsValid)
            {
                try
                {
                    // 2. Busca o veículo ORIGINAL do banco de dados (incluindo dados que não queremos perder)
                    var veiculoBanco = await _context.Veiculos.FindAsync(id);

                    if (veiculoBanco == null)
                    {
                        return NotFound();
                    }

                    // 3. Atualiza manualmente apenas os campos permitidos
                    veiculoBanco.Modelo = veiculoFormulario.Modelo;
                    veiculoBanco.Marca = veiculoFormulario.Marca;
                    veiculoBanco.Preco = veiculoFormulario.Preco;
                    veiculoBanco.Motor = veiculoFormulario.Motor;
                    veiculoBanco.Potencia = veiculoFormulario.Potencia;
                    veiculoBanco.Quilometragem = veiculoFormulario.Quilometragem;
                    veiculoBanco.Ano = veiculoFormulario.Ano;
                    veiculoBanco.Vendido = veiculoFormulario.Vendido;

                    // Nota: Não mexemos em veiculoBanco.Fotos, então as imagens antigas são preservadas!

                    _context.Update(veiculoBanco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoModelExists(veiculoFormulario.Id))
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

            return View(veiculoFormulario);
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

            var veiculoModel = await _context.Veiculos.FirstOrDefaultAsync(m => m.Id == id);
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
