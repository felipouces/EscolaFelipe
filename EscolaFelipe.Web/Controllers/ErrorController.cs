using Microsoft.AspNetCore.Mvc;

namespace EscolaFelipe.Web.Controllers
{
    public class ErrorController : Controller
    {
        // Trata códigos HTTP específicos (404, 500, etc.)
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorMessage"] = "The page you are looking for was not found.";
                    return View("404");

                case 500:
                    ViewData["ErrorMessage"] = "An internal server error occurred.";
                    return View("500");

                default:
                    ViewData["ErrorMessage"] = $"Unexpected error (code {statusCode}).";
                    return View("Error");
            }
        }

        // Erros genéricos (exceções não tratadas)
        [Route("Error/Error")]
        public IActionResult Error()
        {
            ViewData["ErrorMessage"] = "An unexpected error occurred.";
            return View("Error");
        }
    }
}

