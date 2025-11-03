using System;
using System.ComponentModel.DataAnnotations;

namespace EscolaFelipe.Web.Data.Entities
{
    public class Enrollment
    {

        public int Id { get; set; }

        [Display(Name = "Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Display(Name = "Discipline")]
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    }
}
