using EscolaFelipe.Web.Models;  
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace EscolaFelipe.Web.Data  
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Administrador", "Funcionario", "Aluno", "Anonimo" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@escola.pt",
                Email = "admin@escola.pt",
                Nome = "Administrador",
                Morada = "Escola Principal",
                DataNascimento = new DateTime(1980, 1, 1),
                EmailConfirmed = true
            };

            string adminPassword = "Admin123!";
            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }


            // Create funcionário user
            var funcionarioUser = new ApplicationUser
            {
                UserName = "funcionario@escola.pt",
                Email = "funcionario@escola.pt",
                Nome = "Funcionário Teste",
                Morada = "Escola Principal",
                DataNascimento = new DateTime(1985, 1, 1),
                EmailConfirmed = true
            };

            string funcionarioPassword = "Funcionario123!";
            var funcionario = await userManager.FindByEmailAsync(funcionarioUser.Email);

            if (funcionario == null)
            {
                var createFuncionario = await userManager.CreateAsync(funcionarioUser, funcionarioPassword);
                if (createFuncionario.Succeeded)
                {
                    await userManager.AddToRoleAsync(funcionarioUser, "Funcionario");
                }
            }
        }
    }
}
