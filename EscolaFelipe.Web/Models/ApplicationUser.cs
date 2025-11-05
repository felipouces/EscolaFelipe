using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipeTeste.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Morada { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        // Navigation properties - ATUALIZADO para Inscricoes
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}
