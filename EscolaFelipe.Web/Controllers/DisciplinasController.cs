using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize]
    public class DisciplinasController : Controller
    {
        private readonly IDisciplinaRepository _disciplinaRepository;
        private readonly ILogger<DisciplinasController> _logger;

        public DisciplinasController(IDisciplinaRepository disciplinaRepository, ILogger<DisciplinasController> logger)
        {
            _disciplinaRepository = disciplinaRepository;
            _logger = logger;
        }

        // GET: Disciplinas - Todos podem ver (com permissões diferentes)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Anónimos e Alunos veem apenas disciplinas ativas
                if (!User.Identity.IsAuthenticated || User.IsInRole("Aluno") || User.IsInRole("Anonimo"))
                {
                    var disciplinasAtivas = await _disciplinaRepository.GetAtivasAsync();
                    return View(disciplinasAtivas);
                }

                // Admin e Funcionários veem todas
                var todasDisciplinas = await _disciplinaRepository.GetAllAsync();
                return View(todasDisciplinas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de disciplinas");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de disciplinas.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Disciplinas/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da disciplina não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var disciplina = await _disciplinaRepository.GetByIdAsync(id.Value);
                if (disciplina == null)
                {
                    TempData["ErrorMessage"] = "Disciplina não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar se anónimo/aluno tenta ver disciplina inativa
                if (!disciplina.Ativo && (!User.Identity.IsAuthenticated || User.IsInRole("Aluno") || User.IsInRole("Anonimo")))
                {
                    TempData["ErrorMessage"] = "Disciplina não disponível.";
                    return RedirectToAction(nameof(Index));
                }

                return View(disciplina);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da disciplina com ID {DisciplinaId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes da disciplina.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Disciplinas/Create - Apenas Admin
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Disciplinas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Disciplina disciplina)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _disciplinaRepository.AddAsync(disciplina);
                    TempData["SuccessMessage"] = "Disciplina criada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                return View(disciplina);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar disciplina");
                TempData["ErrorMessage"] = $"Erro ao criar disciplina: {ex.Message}";
                return View(disciplina);
            }
        }

        // GET: Disciplinas/Edit/5 - Apenas Admin
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da disciplina não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var disciplina = await _disciplinaRepository.GetByIdAsync(id.Value);
                if (disciplina == null)
                {
                    TempData["ErrorMessage"] = "Disciplina não encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                return View(disciplina);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar disciplina para edição com ID {DisciplinaId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar disciplina para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Disciplinas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Disciplina disciplina)
        {
            if (id != disciplina.DisciplinaId)
            {
                TempData["ErrorMessage"] = "ID da disciplina não corresponde.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _disciplinaRepository.UpdateAsync(disciplina);
                    TempData["SuccessMessage"] = "Disciplina atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                return View(disciplina);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar disciplina com ID {DisciplinaId}", id);
                TempData["ErrorMessage"] = $"Erro ao atualizar disciplina: {ex.Message}";
                return View(disciplina);
            }
        }

        // GET: Disciplinas/Delete/5 - Apenas Admin
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da disciplina não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var disciplina = await _disciplinaRepository.GetByIdAsync(id.Value);
                if (disciplina == null)
                {
                    TempData["ErrorMessage"] = "Disciplina não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(disciplina);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar disciplina para eliminação com ID {DisciplinaId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar disciplina para eliminação.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Disciplinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _disciplinaRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Disciplina eliminada com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao eliminar disciplina com ID {DisciplinaId}", id);
                TempData["ErrorMessage"] = $"Erro ao eliminar disciplina: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
