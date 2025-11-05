using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EscolaFelipeWeb.Controllers
{
    [Authorize]
    public class AlunosController : Controller
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly ILogger<AlunosController> _logger;

        public AlunosController(IAlunoRepository alunoRepository, ILogger<AlunosController> logger)
        {
            _alunoRepository = alunoRepository;
            _logger = logger;
        }

        // GET: Alunos
        public async Task<IActionResult> Index()
        {
            try
            {
                var alunos = await _alunoRepository.GetAllAsync();
                return View(alunos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de alunos");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de alunos. Por favor, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Alunos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID do aluno não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var aluno = await _alunoRepository.GetByIdAsync(id.Value);
                if (aluno == null)
                {
                    TempData["ErrorMessage"] = "Aluno não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(aluno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do aluno com ID {AlunoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes do aluno.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Alunos/Create
        [Authorize(Roles = "Administrador,Funcionario")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Alunos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Funcionario")]
        public async Task<IActionResult> Create(Aluno aluno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _alunoRepository.AddAsync(aluno);
                    TempData["SuccessMessage"] = "Aluno criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                // Se houver erros de validação, mostra a view com os erros
                return View(aluno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluno");
                TempData["ErrorMessage"] = $"Erro ao criar aluno: {ex.Message}";
                return View(aluno);
            }
        }

        // GET: Alunos/Edit/5
        [Authorize(Roles = "Administrador,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID do aluno não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var aluno = await _alunoRepository.GetByIdAsync(id.Value);
                if (aluno == null)
                {
                    TempData["ErrorMessage"] = "Aluno não encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                return View(aluno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aluno para edição com ID {AlunoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar aluno para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Alunos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Funcionario")]
        public async Task<IActionResult> Edit(int id, Aluno aluno)
        {
            if (id != aluno.AlunoId)
            {
                TempData["ErrorMessage"] = "ID do aluno não corresponde.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _alunoRepository.UpdateAsync(aluno);
                    TempData["SuccessMessage"] = "Aluno atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                return View(aluno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno com ID {AlunoId}", id);
                TempData["ErrorMessage"] = $"Erro ao atualizar aluno: {ex.Message}";
                return View(aluno);
            }
        }

        // GET: Alunos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID do aluno não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var aluno = await _alunoRepository.GetByIdAsync(id.Value);
                if (aluno == null)
                {
                    TempData["ErrorMessage"] = "Aluno não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(aluno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aluno para eliminação com ID {AlunoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar aluno para eliminação.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Alunos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _alunoRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Aluno eliminado com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao eliminar aluno com ID {AlunoId}", id);
                TempData["ErrorMessage"] = $"Erro ao eliminar aluno: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}