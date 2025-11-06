using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EscolaFelipe.Web.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IInscricaoRepository _inscricaoRepository;
        private readonly ILogger<PerfilController> _logger;
        private readonly IWebHostEnvironment _environment;

        public PerfilController(
            UserManager<ApplicationUser> userManager,
            IAlunoRepository alunoRepository,
            IInscricaoRepository inscricaoRepository,
            ILogger<PerfilController> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _alunoRepository = alunoRepository;
            _inscricaoRepository = inscricaoRepository;
            _logger = logger;
            _environment = environment;
        }

         // GET: Perfil/MeuPerfil - Aluno vê e edita seu perfil
        [Authorize(Roles = "Aluno,Administrador,Funcionario")]
        public async Task<IActionResult> MeuPerfil()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Utilizador não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                // Buscar o aluno correspondente ao utilizador (apenas para alunos)
                Aluno? aluno = null;
                if (User.IsInRole("Aluno"))
                {
                    var alunos = await _alunoRepository.GetAllAsync();
                    aluno = alunos.FirstOrDefault(a => a.Email == user.Email);
                }

                var viewModel = new PerfilViewModel
                {
                    Nome = user.Nome,
                    Email = user.Email,
                    Morada = user.Morada,
                    DataNascimento = user.DataNascimento,
                    Telefone = user.PhoneNumber ?? "",
                    FotoPerfil = user.FotoPerfil,
                    Aluno = aluno
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfil do utilizador");
                TempData["ErrorMessage"] = "Erro ao carregar perfil.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Perfil/MeuPerfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Aluno,Administrador,Funcionario")]
        public async Task<IActionResult> MeuPerfil(PerfilViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Utilizador não encontrado.";
                        return RedirectToAction("Index", "Home");
                    }

                    // Processar upload da foto se existir
                    if (viewModel.FotoFile != null && viewModel.FotoFile.Length > 0)
                    {
                        var fileName = await ProcessarUploadFoto(viewModel.FotoFile, user.Id);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            user.FotoPerfil = fileName;
                        }
                    }

                    // Atualizar dados do utilizador
                    user.Nome = viewModel.Nome;
                    user.Morada = viewModel.Morada;
                    user.DataNascimento = viewModel.DataNascimento;
                    user.PhoneNumber = viewModel.Telefone;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        // Atualizar também o aluno se existir e for um aluno
                        if (User.IsInRole("Aluno"))
                        {
                            var alunos = await _alunoRepository.GetAllAsync();
                            var aluno = alunos.FirstOrDefault(a => a.Email == user.Email);
                            if (aluno != null)
                            {
                                aluno.Nome = viewModel.Nome;
                                aluno.Morada = viewModel.Morada;
                                aluno.DataNascimento = viewModel.DataNascimento;
                                aluno.Telefone = viewModel.Telefone;

                                await _alunoRepository.UpdateAsync(aluno);
                            }
                        }

                        TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                        return RedirectToAction(nameof(MeuPerfil));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do utilizador");
                TempData["ErrorMessage"] = $"Erro ao atualizar perfil: {ex.Message}";
                return View(viewModel);
            }
        }

        // Método para processar o upload da foto
        private async Task<string?> ProcessarUploadFoto(IFormFile fotoFile, string userId)
        {
            try
            {
                // Validar o tipo de ficheiro
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fotoFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("FotoFile", "Apenas são permitidos ficheiros JPG, JPEG, PNG ou GIF.");
                    return null;
                }

                // Validar o tamanho do ficheiro (max 5MB)
                if (fotoFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("FotoFile", "O ficheiro não pode exceder 5MB.");
                    return null;
                }

                // Criar pasta de uploads se não existir
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "fotos-perfil");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Gerar nome único para o ficheiro
                var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar o ficheiro
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fotoFile.CopyToAsync(stream);
                }

                return $"/uploads/fotos-perfil/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar upload da foto");
                ModelState.AddModelError("FotoFile", "Erro ao processar a foto.");
                return null;
            }
        }


        // GET: Perfil/RemoverFoto
        [Authorize(Roles = "Aluno,Administrador,Funcionario")]
        public async Task<IActionResult> RemoverFoto()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Utilizador não encontrado.";
                    return RedirectToAction(nameof(MeuPerfil));
                }

                // Remover ficheiro físico se existir
                if (!string.IsNullOrEmpty(user.FotoPerfil))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, user.FotoPerfil.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                user.FotoPerfil = null;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Foto de perfil removida com sucesso!";
                return RedirectToAction(nameof(MeuPerfil));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover foto de perfil");
                TempData["ErrorMessage"] = "Erro ao remover foto de perfil.";
                return RedirectToAction(nameof(MeuPerfil));
            }
        }



        // GET: Perfil/MinhasInscricoes - Aluno vê apenas suas inscrições
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> MinhasInscricoes()
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
                    return View(new List<Inscricao>());
                }

                var minhasInscricoes = await _inscricaoRepository.GetByAlunoIdAsync(aluno.AlunoId);
                return View(minhasInscricoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar inscrições do aluno");
                TempData["ErrorMessage"] = "Erro ao carregar inscrições.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Perfil/DetalhesInscricao/5 - Aluno vê detalhes da sua inscrição
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> DetalhesInscricao(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID da inscrição não especificado.";
                return RedirectToAction(nameof(MinhasInscricoes));
            }

            try
            {
                var inscricao = await _inscricaoRepository.GetByIdAsync(id.Value);
                if (inscricao == null)
                {
                    TempData["ErrorMessage"] = "Inscrição não encontrada.";
                    return RedirectToAction(nameof(MinhasInscricoes));
                }

                // Verificar se a inscrição pertence ao aluno atual
                var user = await _userManager.GetUserAsync(User);
                var alunos = await _alunoRepository.GetAllAsync();
                var aluno = alunos.FirstOrDefault(a => a.Email == user.Email);

                if (aluno == null || inscricao.AlunoId != aluno.AlunoId)
                {
                    TempData["ErrorMessage"] = "Não tem permissão para ver esta inscrição.";
                    return RedirectToAction(nameof(MinhasInscricoes));
                }

                return View(inscricao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da inscrição com ID {InscricaoId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes da inscrição.";
                return RedirectToAction(nameof(MinhasInscricoes));
            }
        }
    }
}