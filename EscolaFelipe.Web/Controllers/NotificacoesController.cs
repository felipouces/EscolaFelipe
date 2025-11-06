using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize]
    public class NotificacoesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly INotificacaoService _notificacaoService;
        private readonly ILogger<NotificacoesController> _logger;

        public NotificacoesController(
            UserManager<ApplicationUser> userManager,
            INotificacaoRepository notificacaoRepository,
            INotificacaoService notificacaoService,
            ILogger<NotificacoesController> logger)
        {
            _userManager = userManager;
            _notificacaoRepository = notificacaoRepository;
            _notificacaoService = notificacaoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var notificacoes = await _notificacaoRepository.GetByUtilizadorIdAsync(user.Id);

            return View(notificacoes);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            await _notificacaoRepository.MarcarComoLidaAsync(id);

            // Retorna o count atualizado para atualizar o badge
            var user = await _userManager.GetUserAsync(User);
            var countNaoLidas = await _notificacaoRepository.CountNaoLidasByUtilizadorIdAsync(user.Id);

            return Json(new { success = true, countNaoLidas });
        }

        [HttpPost]
        public async Task<IActionResult> MarcarTodasComoLidas()
        {
            var user = await _userManager.GetUserAsync(User);
            await _notificacaoRepository.MarcarTodasComoLidasAsync(user.Id);

            return Json(new { success = true, countNaoLidas = 0 });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificacoesNaoLidas()
        {
            var user = await _userManager.GetUserAsync(User);
            var notificacoes = await _notificacaoRepository.GetNaoLidasByUtilizadorIdAsync(user.Id);
            var countNaoLidas = await _notificacaoRepository.CountNaoLidasByUtilizadorIdAsync(user.Id);

            return Json(new { notificacoes, countNaoLidas });
        }
    }
}