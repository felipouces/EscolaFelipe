using EscolaFelipe.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EscolaFelipe.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (statusCode.HasValue)
            {
                errorViewModel.StatusCode = statusCode.Value;

                switch (statusCode.Value)
                {
                    case 404:
                        errorViewModel.Message = "Página não encontrada.";
                        errorViewModel.Title = "Recurso Não Encontrado";
                        break;
                    case 403:
                        errorViewModel.Message = "Não tem permissões para aceder a este recurso.";
                        errorViewModel.Title = "Acesso Negado";
                        break;
                    case 500:
                        errorViewModel.Message = "Ocorreu um erro interno no servidor.";
                        errorViewModel.Title = "Erro do Servidor";
                        break;
                    default:
                        errorViewModel.Message = "Ocorreu um erro inesperado.";
                        errorViewModel.Title = "Erro";
                        break;
                }
            }
            else
            {
                // Capturar a exceção original
                var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    _logger.LogError(exceptionHandlerPathFeature.Error, "Erro não tratado: {Path}", exceptionHandlerPathFeature.Path);
                    errorViewModel.Message = "Ocorreu um erro inesperado. Por favor, tente novamente.";
                    errorViewModel.Title = "Erro";
                }
            }

            return View(errorViewModel);
        }
    }
}