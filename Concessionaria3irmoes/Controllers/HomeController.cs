using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Concessionaria3irmoes.Models;
using Concessionaria3irmoes.Data;
using Microsoft.EntityFrameworkCore;

namespace Concessionaria3irmoes.Controllers;

public class HomeController : Controller
{
    /// Controlador da Página Inicial (Home).
/// Responsável por exibir a vitrine de veículos e páginas institucionais (Privacidade).
/// Qualquer usuário (logado ou não) pode acessar esta área
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // 1. Adicionar o Contexto

    // 2. Injetar o Contexto no Construtor
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    // GET: / (Raiz do site)
    // Carrega os veículos em destaque para a vitrine inicial.
    public async Task<IActionResult> Index()
    {
        // 3. Buscar os veículos no banco (Pegar os 8 primeiros, por exemplo)
        var veiculosEmDestaque = await _context.Veiculos
                                      .Where(v => !v.Vendido)
                                      .OrderByDescending(v => v.Id) // Mostra os mais novos primeiro
                                      .Take(8) // Limita a 8 carros na home
                                      .ToListAsync();

        // 4. Enviar a lista para a View
        return View(veiculosEmDestaque);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    // Gerenciamento de Erros
    // Exibe uma página amigável caso ocorra algum problema na requisição.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}