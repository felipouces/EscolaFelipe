using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IInscricaoRepository _inscricaoRepository;
        private readonly IDisciplinaRepository _disciplinaRepository;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IAlunoRepository alunoRepository,
            IInscricaoRepository inscricaoRepository,
            IDisciplinaRepository disciplinaRepository,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _alunoRepository = alunoRepository;
            _inscricaoRepository = inscricaoRepository;
            _disciplinaRepository = disciplinaRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Utilizador não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                // Buscar o aluno correspondente ao utilizador
                var alunos = await _alunoRepository.GetAllAsync();
                var aluno = alunos.FirstOrDefault(a => a.Email == user.Email);

                if (aluno == null)
                {
                    TempData["ErrorMessage"] = "Aluno não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                var viewModel = new DashboardAlunoViewModel
                {
                    Aluno = aluno,
                    TotalInscricoes = await _inscricaoRepository.CountInscricoesByAlunoAsync(aluno.AlunoId),
                    InscricoesAtivas = await _inscricaoRepository.CountInscricoesAtivasByAlunoAsync(aluno.AlunoId),
                    InscricoesConcluidas = await _inscricaoRepository.CountInscricoesConcluidasByAlunoAsync(aluno.AlunoId),
                    TotalInvestido = await _inscricaoRepository.GetTotalInvestidoByAlunoAsync(aluno.AlunoId),
                    DisciplinasRecomendadas = await _inscricaoRepository.GetDisciplinasRecomendadasAsync(aluno.AlunoId)
                };

                // Buscar as 3 inscrições mais recentes
                var todasInscricoes = await _inscricaoRepository.GetByAlunoIdAsync(aluno.AlunoId);
                viewModel.ProximasInscricoes = todasInscricoes
                    .Where(i => i.Estado == "Ativa")
                    .OrderBy(i => i.DataInscricao)
                    .Take(3);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard do aluno");
                TempData["ErrorMessage"] = "Erro ao carregar dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}