using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize]
    public class InscricoesController : Controller
    {
        private readonly IInscricaoRepository _inscricaoRepository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IDisciplinaRepository _disciplinaRepository;
        private readonly ILogger<InscricoesController> _logger;

        public InscricoesController(
            IInscricaoRepository inscricaoRepository,
            IAlunoRepository alunoRepository,
            IDisciplinaRepository disciplinaRepository,
            ILogger<InscricoesController> logger)
        {
            _inscricaoRepository = inscricaoRepository;
            _alunoRepository = alunoRepository;
            _disciplinaRepository = disciplinaRepository;
            _logger = logger;
        }

        // GET: Inscricoes - Funcionários veem todas, Alunos veem apenas as suas
        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.IsInRole("Aluno"))
                {
                    // Aluno vê apenas suas próprias inscrições
                    var aluno = await _alunoRepository.GetAllAsync();
                    var alunoDoUsuario = aluno.FirstOrDefault(a => a.Email == User.Identity.Name);

                    if (alunoDoUsuario != null)
                    {
                        var minhasInscricoes = await _inscricaoRepository.GetByAlunoIdAsync(alunoDoUsuario.AlunoId);
                        return View(minhasInscricoes);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Aluno não encontrado.";
                        return View(new List<Inscricao>());
                    }
                }
                else if (User.IsInRole("Funcionario") || User.IsInRole("Administrador"))
                {
                    // Funcionários e Admin veem todas as inscrições
                    var todasInscricoes = await _inscricaoRepository.GetAllAsync();
                    return View(todasInscricoes);
                }
                else
                {
                    TempData["ErrorMessage"] = "Não tem permissão para ver inscrições.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de inscrições");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de inscrições.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Inscricoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da inscrição não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var inscricao = await _inscricaoRepository.GetByIdAsync(id.Value);
                if (inscricao == null)
                {
                    TempData["ErrorMessage"] = "Inscrição não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar permissões
                if (User.IsInRole("Aluno"))
                {
                    var aluno = await _alunoRepository.GetAllAsync();
                    var alunoDoUsuario = aluno.FirstOrDefault(a => a.Email == User.Identity.Name);
                    if (alunoDoUsuario == null || inscricao.AlunoId != alunoDoUsuario.AlunoId)
                    {
                        TempData["ErrorMessage"] = "Não tem permissão para ver esta inscrição.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                return View(inscricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da inscrição com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes da inscrição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inscricoes/Create - Apenas Funcionários e Admin
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await CarregarListas();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para criar inscrição");
                TempData["ErrorMessage"] = "Erro ao carregar dados para criar inscrição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inscricoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> Create(InscricaoCreateViewModel viewModel)
        {
            _logger.LogInformation("=== INÍCIO: Tentativa de criar inscrição ===");
            _logger.LogInformation("AlunoId: {AlunoId}, DisciplinaId: {DisciplinaId}, Estado: {Estado}",
                viewModel.AlunoId, viewModel.DisciplinaId, viewModel.Estado);

            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("ModelState é válido - prosseguindo com criação");

                    // Verificar se o aluno já está inscrito nesta disciplina
                    var jaInscrito = await _inscricaoRepository.AlunoInscritoEmDisciplinaAsync(viewModel.AlunoId, viewModel.DisciplinaId);
                    _logger.LogInformation("Aluno já inscrito: {JaInscrito}", jaInscrito);

                    if (jaInscrito)
                    {
                        ModelState.AddModelError("", "Este aluno já está inscrito nesta disciplina.");
                        TempData["ErrorMessage"] = "Este aluno já está inscrito nesta disciplina.";
                    }
                    else
                    {
                        // Criar a inscrição a partir do ViewModel
                        var inscricao = new Inscricao
                        {
                            AlunoId = viewModel.AlunoId,
                            DisciplinaId = viewModel.DisciplinaId,
                            Estado = viewModel.Estado,
                            ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            DataInscricao = DateTime.Now
                        };

                        _logger.LogInformation("ApplicationUserId definido: {UserId}", inscricao.ApplicationUserId);

                        await _inscricaoRepository.AddAsync(inscricao);
                        _logger.LogInformation("Inscrição criada com sucesso - ID: {InscricaoId}", inscricao.InscricaoId);

                        TempData["SuccessMessage"] = "Inscrição criada com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    _logger.LogWarning("ModelState inválido. Erros:");
                    foreach (var error in ModelState)
                    {
                        foreach (var err in error.Value.Errors)
                        {
                            _logger.LogWarning(" - {Key}: {Error}", error.Key, err.ErrorMessage);
                        }
                    }
                }

                // Recarregar as listas em caso de erro
                _logger.LogInformation("Recarregando listas de alunos e disciplinas");
                await CarregarListas();

                _logger.LogInformation("=== FIM: Processamento de criação de inscrição ===");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERRO ao criar inscrição");
                TempData["ErrorMessage"] = $"Erro ao criar inscrição: {ex.Message}";

                // Recarregar as listas
                await CarregarListas();

                return View(viewModel);
            }
        }

        // GET: Inscricoes/Edit/5 - Apenas Funcionários e Admin
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da inscrição não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var inscricao = await _inscricaoRepository.GetByIdAsync(id.Value);
                if (inscricao == null)
                {
                    TempData["ErrorMessage"] = "Inscrição não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarListas();
                return View(inscricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar inscrição para edição com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar inscrição para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inscricoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("InscricaoId,AlunoId,DisciplinaId,Estado,ApplicationUserId,DataInscricao")] Inscricao inscricao)
        {
            if (id != inscricao.InscricaoId)
            {
                TempData["ErrorMessage"] = "ID da inscrição não corresponde.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _inscricaoRepository.UpdateAsync(inscricao);
                    TempData["SuccessMessage"] = "Inscrição atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarListas();
                return View(inscricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar inscrição com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = $"Erro ao atualizar inscrição: {ex.Message}";

                await CarregarListas();
                return View(inscricao);
            }
        }

        // GET: Inscricoes/Delete/5 - Apenas Funcionários e Admin
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da inscrição não especificado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var inscricao = await _inscricaoRepository.GetByIdAsync(id.Value);
                if (inscricao == null)
                {
                    TempData["ErrorMessage"] = "Inscrição não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(inscricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar inscrição para eliminação com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar inscrição para eliminação.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inscricoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _inscricaoRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Inscrição eliminada com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao eliminar inscrição com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = $"Erro ao eliminar inscrição: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Teste de criação de inscrição (apenas para debug)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> TesteInscricao()
        {
            try
            {
                // Criar uma inscrição manualmente para teste
                var alunos = await _alunoRepository.GetAllAsync();
                var disciplinas = await _disciplinaRepository.GetAtivasAsync();

                if (alunos.Any() && disciplinas.Any())
                {
                    var inscricaoTeste = new Inscricao
                    {
                        AlunoId = alunos.First().AlunoId,
                        DisciplinaId = disciplinas.First().DisciplinaId,
                        Estado = "Ativa",
                        ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };

                    await _inscricaoRepository.AddAsync(inscricaoTeste);
                    TempData["SuccessMessage"] = "Inscrição de teste criada com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não há alunos ou disciplinas para testar";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro no teste: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para carregar as listas
        private async Task CarregarListas()
        {
            var alunos = await _alunoRepository.GetAllAsync();
            var disciplinas = await _disciplinaRepository.GetAtivasAsync();
            ViewData["AlunoId"] = new SelectList(alunos, "AlunoId", "Nome");
            ViewData["DisciplinaId"] = new SelectList(disciplinas, "DisciplinaId", "Nome");
        }
    }
}