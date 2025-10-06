using EscolaFelipe.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using EscolaFelipe.Web.Services;



namespace EscolaFelipe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Only administrators can access this controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender; // To send emails

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        // List of existing users
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // New user creation form
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // User creation (POST)
        [HttpPost]
        public async Task<IActionResult> CreateUser(string fullName, string email, string role)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            // Check if role exists, if not, create it
            // Checks if the user already exists in the system
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "User with this email already exists.");
                return View();
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var tempPassword = Guid.NewGuid().ToString().Substring(0, 8) + "Aa!";
            // temporary password
            var result = await _userManager.CreateAsync(user, tempPassword);

            if (result.Succeeded)
            {
                // Adiciona role
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);

                // Gera token e link de reset de password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Envia email
                await _emailSender.SendEmailAsync(email,
                    "Ative sua conta - Escola Web",
                    $"Olá {fullName},<br/>Foi criada uma conta para si na plataforma Escola Web.<br/>" +
                    $"Por favor, defina uma nova password clicando neste link:<br/>" +
                    $"<a href='{resetLink}'>Alterar password</a>");

                TempData["Message"] = "User created successfully and password reset email sent.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();

        }

        // Delete user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                TempData["Message"] = "User deleted successfully.";
            else
                TempData["Error"] = "Error deleting user.";

            return RedirectToAction(nameof(Index));
        }

    }
}
