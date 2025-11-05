using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EscolaFelipe.Web.Models
{
    public class Inscricao
    {
        [Key]
        public int InscricaoId { get; set; }

        [Required(ErrorMessage = "O campo Aluno é obrigatório.")]
        [Display(Name = "Aluno")]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "O campo Disciplina é obrigatório.")]
        [Display(Name = "Disciplina")]
        public int DisciplinaId { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Data de Inscrição")]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Ativa";

        // Navigation properties - IGNORAR na validação
        [JsonIgnore]
        [ValidateNever]
        public virtual Aluno Aluno { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual Disciplina Disciplina { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}