using EscolaFelipe.Web.Data;
using EscolaFelipe.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EscolaFelipe.Web.Controllers
{

    [Authorize(Roles = "Funcionario,Aluno")]
    public class EnrollmentsController : Controller
    {

        private readonly DataContext _context;

        public EnrollmentsController(DataContext context)
        {
            _context = context;
        }

        // FUNCIONÁRIO: Gerir inscrições (CRUD normal)
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Index()
        {
            var registrations = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Discipline)
                .ToListAsync();
            return View(registrations);
        }

        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Discipline)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        [Authorize(Roles = "Funcionario")]
        public IActionResult Create()
        {
            ViewData["DisciplineId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Disciplines, "Id", "Name");
            ViewData["StudentId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Students, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Create([Bind("Id,StudentId,DisciplineId,RegistrationDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DisciplineId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Disciplines, "Id", "Name", enrollment.DisciplineId);
            ViewData["StudentId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            ViewData["DisciplineId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Disciplines, "Id", "Name", enrollment.DisciplineId);
            ViewData["StudentId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,DisciplineId,RegistrationDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Enrollments.Any(e => e.Id == enrollment.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DisciplineId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Disciplines, "Id", "Name", enrollment.DisciplineId);
            ViewData["StudentId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Students, "Id", "Name", enrollment.StudentId);
            return View(enrollment);
        }

        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Discipline)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // ALUNO: Ver as SUAS inscrições (N:N)
        // ======================================================
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> MyEnrollments()
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            // Busca o aluno pelo e-mail (assumindo que Student.Email == ApplicationUser.Email)
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
                return NotFound("Student not found for logged user.");

            // Lista as inscrições do aluno
            var enrollments = await _context.Enrollments
                .Include(e => e.Discipline)
                .Where(e => e.StudentId == student.Id)
                .ToListAsync();

            return View(enrollments);
        }

    }
}
