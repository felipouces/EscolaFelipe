using EscolaFelipe.Web.Data;
using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using Microsoft.EntityFrameworkCore;

namespace EscolaFelipe.Web.Repository
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notificacao>> GetByUtilizadorIdAsync(string utilizadorId)
        {
            return await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notificacao>> GetNaoLidasByUtilizadorIdAsync(string utilizadorId)
        {
            return await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId && !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .Take(10)
                .ToListAsync();
        }

        public async Task<int> CountNaoLidasByUtilizadorIdAsync(string utilizadorId)
        {
            return await _context.Notificacoes
                .CountAsync(n => n.UtilizadorId == utilizadorId && !n.Lida);
        }

        public async Task AddAsync(Notificacao notificacao)
        {
            await _context.Notificacoes.AddAsync(notificacao);
            await _context.SaveChangesAsync();
        }

        public async Task MarcarComoLidaAsync(int notificacaoId)
        {
            var notificacao = await _context.Notificacoes.FindAsync(notificacaoId);
            if (notificacao != null)
            {
                notificacao.Lida = true;
                notificacao.DataLeitura = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarcarTodasComoLidasAsync(string utilizadorId)
        {
            var notificacoes = await _context.Notificacoes
                .Where(n => n.UtilizadorId == utilizadorId && !n.Lida)
                .ToListAsync();

            foreach (var notificacao in notificacoes)
            {
                notificacao.Lida = true;
                notificacao.DataLeitura = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int notificacaoId)
        {
            var notificacao = await _context.Notificacoes.FindAsync(notificacaoId);
            if (notificacao != null)
            {
                _context.Notificacoes.Remove(notificacao);
                await _context.SaveChangesAsync();
            }
        }
    }
}