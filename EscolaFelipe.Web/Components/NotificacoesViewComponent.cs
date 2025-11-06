using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EscolaFelipe.Web.Components
{
    public class NotificacoesViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificacaoRepository _notificacaoRepository;

        public NotificacoesViewComponent(
            UserManager<ApplicationUser> userManager,
            INotificacaoRepository notificacaoRepository)
        {
            _userManager = userManager;
            _notificacaoRepository = notificacaoRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return Content(string.Empty);

            var notificacoes = await _notificacaoRepository.GetNaoLidasByUtilizadorIdAsync(user.Id);
            var countNaoLidas = await _notificacaoRepository.CountNaoLidasByUtilizadorIdAsync(user.Id);

            ViewBag.CountNaoLidas = countNaoLidas;
            return View(notificacoes);
        }
    }
}