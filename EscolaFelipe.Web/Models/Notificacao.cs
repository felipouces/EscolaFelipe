using EscolaFelipe.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscolaFelipe.Web.Models
{
    public class Notificacao
    {
        [Key]
        public int NotificacaoId { get; set; }

        [Required]
        public string UtilizadorId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = "Info"; // Info, Success, Warning, Danger

        public bool Lida { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? DataLeitura { get; set; }

        public string UrlAction { get; set; } = string.Empty;
        public string UrlController { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("UtilizadorId")]
        public virtual ApplicationUser Utilizador { get; set; } = null!;
    }
}
