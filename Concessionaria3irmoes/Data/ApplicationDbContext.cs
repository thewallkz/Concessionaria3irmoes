using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Concessionaria3irmoes.Models;

namespace Concessionaria3irmoes.Data;

/// Contexto principal do Banco de Dados.
/// Herda de IdentityDbContext para incluir automaticamente as tabelas de usuários e segurança (Login/Roles).
public class ApplicationDbContext : IdentityDbContext
{
    // Construtor que recebe as opções de configuração (como a String de Conexão) do Program.cs
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // --- Mapeamento das Tabelas (DbSet) ---
    // Cada propriedade aqui vira uma tabela no banco de dados.
    public DbSet<VeiculoModel> Veiculos { get; set; }
    public DbSet<ClienteModel> Clientes { get; set; }
    public DbSet<VendaModel> Vendas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // IMPORTANTE: O base.OnModelCreating é obrigatório para o Identity funcionar.
        // Sem ele, as tabelas de usuários (AspNetUsers, etc) não são criadas corretamente.
        base.OnModelCreating(builder);

        // Configuração dos decimais (Preço e ValorFinal)
        builder.Entity<VeiculoModel>()
            .Property(v => v.Preco)
            .HasPrecision(18, 2);

        builder.Entity<VendaModel>()
            .Property(v => v.ValorFinal)
            .HasPrecision(18, 2);
    }
}