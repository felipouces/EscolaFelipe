using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Data.Entities
{
    public class Student
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Relação N:N via Inscricao
        public ICollection<Enrollment> Registrations { get; set; }

    }
}
