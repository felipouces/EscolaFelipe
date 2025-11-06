using EscolaFelipe.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.ViewModels
{
    public class PerfilViewModel
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Por favor, insira um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Morada é obrigatório.")]
        [StringLength(200, ErrorMessage = "A morada não pode ter mais de 200 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
        [Phone(ErrorMessage = "Por favor, insira um número de telefone válido.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        // Campo para upload de foto
        [Display(Name = "Foto de Perfil")]
        public IFormFile? FotoFile { get; set; }

        public string? FotoPerfil { get; set; }

        // Dados do aluno (apenas para exibição)
        public Aluno? Aluno { get; set; }
    }
}
