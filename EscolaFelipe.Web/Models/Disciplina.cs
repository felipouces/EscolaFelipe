using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Models
{
    public class Disciplina
    {
        [Key]
        public int DisciplinaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Duração (horas)")]
        public int DuracaoHoras { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [Display(Name = "Disponível")]
        public bool Ativo { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}