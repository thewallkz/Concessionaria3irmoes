using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Concessionaria3irmoes.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Cria o Papel de "Admin" se não existir
            string[] roleNames = { "Admin", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Cria o Usuário Admin (admin@loja.com)
            var emailAdmin = "admin@loja.com";
            var usuarioAdmin = await userManager.FindByEmailAsync(emailAdmin);

            if (usuarioAdmin == null)
            {
                usuarioAdmin = new IdentityUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true 
                };

                
                await userManager.CreateAsync(usuarioAdmin, "Senha@123");
                
                // Adiciona o usuário ao papel de Admin
                await userManager.AddToRoleAsync(usuarioAdmin, "Admin");
            }
            
            // Cria um Usuário Cliente para testes
            var emailCliente = "cliente@gmail.com";
            var usuarioCliente = await userManager.FindByEmailAsync(emailCliente);

            if (usuarioCliente == null)
            {
                usuarioCliente = new IdentityUser
                {
                    UserName = emailCliente,
                    Email = emailCliente,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(usuarioCliente, "Senha@123");
                
            }
        }
    }
}