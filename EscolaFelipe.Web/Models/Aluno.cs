using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Models
{
    public class Aluno
    {
        [Key]
        public int AlunoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(200)]
        public string Morada { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        public DateTime DataInscricao { get; set; } = DateTime.Now;

        // Navigation properties - ATUALIZADO para Inscricoes
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}