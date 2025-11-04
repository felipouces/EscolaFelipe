using EscolaFelipe.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EscolaFelipe.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(
            DataContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            // Cria as roles se não existirem
            await CheckRoleAsync("Admin");
            await CheckRoleAsync("Funcionario");
            await CheckRoleAsync("Aluno");

            // Cria utilizadores
            var admin = await CheckUserAsync("admin@escola.com", "Admin User", "Admin");
            var func = await CheckUserAsync("func@escola.com", "Funcionario User", "Funcionario");
            var aluno = await CheckUserAsync("aluno@escola.com", "Aluno Teste", "Aluno");

            // Cria disciplinas iniciais
            await CheckDisciplinesAsync();

            // Cria aluno na tabela Students associado ao utilizador aluno
            await CheckStudentAsync(aluno.Email, aluno.FullName);
        }

        private async Task CheckRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }
        }

        private async Task<ApplicationUser> CheckUserAsync(string email, string fullName, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Escola@123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Não foi possível criar o utilizador {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await _userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        private async Task CheckDisciplinesAsync()
        {
            if (!_context.Disciplines.Any())
            {
                _context.Disciplines.AddRange(
                    new Discipline { Name = "Matemática" },
                    new Discipline { Name = "Português" },
                    new Discipline { Name = "História" },
                    new Discipline { Name = "Informática" }
                );

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckStudentAsync(string email, string fullName)
        {
            if (!_context.Students.Any(s => s.Email == email))
            {
                var student = new Student
                {
                    Name = fullName,
                    Email = email,
                    DateOfBirth = new DateTime(2000, 1, 1)
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}
