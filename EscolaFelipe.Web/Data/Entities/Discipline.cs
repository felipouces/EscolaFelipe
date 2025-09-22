using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Data.Entities
{
    public class Discipline
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [Display(Name = "Discipline Name")]
        public string Name { get; set; }


        // Relação N:N via Inscricao
        public ICollection<Enrollment> Registrations { get; set; }

    }
}
