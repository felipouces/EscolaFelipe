using EscolaFelipe.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EscolaFelipe.Web.Data
{
    public class DataContext
    {

        public EscolaWebContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Enrollment> Registration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração das relações N:N (via Inscrições)
            modelBuilder.Entity<Enrollment>()
                .HasOne(i => i.Aluno)
                .WithMany(a => a.Inscricoes)
                .HasForeignKey(i => i.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(i => i.Disciplina)
                .WithMany(d => d.Inscricoes)
                .HasForeignKey(i => i.DisciplinaId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
