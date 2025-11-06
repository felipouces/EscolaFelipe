using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.ViewModels
{
    public class InscricaoCreateViewModel
    {
        [Required(ErrorMessage = "O campo Aluno é obrigatório.")]
        [Display(Name = "Aluno")]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "O campo Disciplina é obrigatório.")]
        [Display(Name = "Disciplina")]
        public int DisciplinaId { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Ativa";
    }
}