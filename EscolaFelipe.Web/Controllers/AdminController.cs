using EscolaFelipe.Web.Data.Entities;
using EscolaFelipe.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Only administrators can access this controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _env = env;
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
        public async Task<IActionResult> CreateUser(string fullName, string email, string role, IFormFile photo)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            // Checks if the user already exists in the system
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "User with this email already exists.");
                return View();
            }

            // Process photo upload (if provided)
            string photoUrl = null;
            if (photo != null && photo.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "images/users");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{photo.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                photoUrl = $"/images/users/{fileName}";
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                PhotoProfileUrl = photoUrl,
                EmailConfirmed = true
            };

            var tempPassword = Guid.NewGuid().ToString().Substring(0, 8) + "Aa!";
            var result = await _userManager.CreateAsync(user, tempPassword);

            if (result.Succeeded)
            {
                // Add role (create if not exists)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);

                // Generate reset password link
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Send email
                await _emailSender.SendEmailAsync(email,
                    "Ative sua conta - Escola Web",
                    $"Olá {fullName},<br/>" +
                    $"Foi criada uma conta para si na plataforma Escola Web.<br/>" +
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
