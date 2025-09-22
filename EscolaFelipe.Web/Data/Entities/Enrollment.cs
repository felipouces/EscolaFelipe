using System;

namespace EscolaFelipe.Web.Data.Entities
{
    public class Enrollment
    {

        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int DisciplineId { get; set; }

        public Discipline Discipline { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

    }
}
