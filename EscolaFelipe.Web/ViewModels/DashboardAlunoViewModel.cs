using EscolaFelipe.Web.Models;

namespace EscolaFelipe.Web.ViewModels
{
    public class DashboardAlunoViewModel
    {
        public int TotalInscricoes { get; set; }
        public int InscricoesAtivas { get; set; }
        public int InscricoesConcluidas { get; set; }
        public decimal TotalInvestido { get; set; }
        public IEnumerable<Inscricao> ProximasInscricoes { get; set; } = new List<Inscricao>();
        public IEnumerable<Disciplina> DisciplinasRecomendadas { get; set; } = new List<Disciplina>();
        public Aluno? Aluno { get; set; }

        // Métodos auxiliares para a view
        public double PercentualConcluido => TotalInscricoes > 0 ?
            (InscricoesConcluidas * 100.0) / TotalInscricoes : 0;

        public double PercentualAtivo => TotalInscricoes > 0 ?
            (InscricoesAtivas * 100.0) / TotalInscricoes : 0;
    }
}