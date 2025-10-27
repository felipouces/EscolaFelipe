using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Display(Name = "Profile photo")]
        public string PhotoProfileUrl { get; set; }

    }
}
