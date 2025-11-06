using EscolaFelipe.Web.Models;

namespace EscolaFelipe.Web.Services
{
    public interface INotificacaoService
    {
        Task NotificarNovoAlunoAsync(Aluno aluno, string utilizadorId);
        Task NotificarNovaInscricaoAsync(Inscricao inscricao);
        Task NotificarAtualizacaoDisciplinaAsync(Disciplina disciplina);
        Task NotificarAproximacaoFimInscricaoAsync();
        Task NotificarAchievementAsync(string utilizadorId, string achievement);
    }
}