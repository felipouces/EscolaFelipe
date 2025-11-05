using EscolaFelipe.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EscolaFelipe.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships - N para N entre Aluno e Disciplina através de Inscricao
            builder.Entity<Inscricao>()
                .HasOne(i => i.Aluno)
                .WithMany(a => a.Inscricoes)
                .HasForeignKey(i => i.AlunoId);

            builder.Entity<Inscricao>()
                .HasOne(i => i.Disciplina)
                .WithMany(d => d.Inscricoes)
                .HasForeignKey(i => i.DisciplinaId);

            builder.Entity<Inscricao>()
                .HasOne(i => i.ApplicationUser)
                .WithMany(u => u.Inscricoes)
                .HasForeignKey(i => i.ApplicationUserId);
        }
    }
}