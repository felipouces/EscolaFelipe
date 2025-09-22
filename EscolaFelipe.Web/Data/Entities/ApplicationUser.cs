using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }


        [Display(Name = "Foto de Perfil")]
        public string PhotoProfileUrl { get; set; }

    }
}
