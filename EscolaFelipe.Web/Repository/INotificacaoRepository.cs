using EscolaFelipe.Web.Models;


namespace EscolaFelipe.Web.Repository
{
    public interface INotificacaoRepository
    {
        Task<IEnumerable<Notificacao>> GetByUtilizadorIdAsync(string utilizadorId);
        Task<IEnumerable<Notificacao>> GetNaoLidasByUtilizadorIdAsync(string utilizadorId);
        Task<int> CountNaoLidasByUtilizadorIdAsync(string utilizadorId);
        Task AddAsync(Notificacao notificacao);
        Task MarcarComoLidaAsync(int notificacaoId);
        Task MarcarTodasComoLidasAsync(string utilizadorId);
        Task DeleteAsync(int notificacaoId);
    }
}
