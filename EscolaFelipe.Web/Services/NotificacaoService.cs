using EscolaFelipe.Web.Models;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.Services;


namespace EscolaFelipe.Web.Services
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly IInscricaoRepository _inscricaoRepository;
        private readonly ILogger<NotificacaoService> _logger;

        public NotificacaoService(
            INotificacaoRepository notificacaoRepository,
            IInscricaoRepository inscricaoRepository,
            ILogger<NotificacaoService> logger)
        {
            _notificacaoRepository = notificacaoRepository;
            _inscricaoRepository = inscricaoRepository;
            _logger = logger;
        }

        public async Task NotificarNovoAlunoAsync(Aluno aluno, string utilizadorId)
        {
            var notificacao = new Notificacao
            {
                UtilizadorId = utilizadorId,
                Titulo = "Bem-vindo à EscolaFelipe! 🎓",
                Mensagem = $"Olá {aluno.Nome}! A sua conta foi criada com sucesso. Explore as disciplinas disponíveis.",
                Tipo = "Success",
                UrlController = "Disciplinas",
                UrlAction = "Index"
            };

            await _notificacaoRepository.AddAsync(notificacao);
            _logger.LogInformation("Notificação de boas-vindas enviada para {AlunoNome}", aluno.Nome);
        }

        public async Task NotificarNovaInscricaoAsync(Inscricao inscricao)
        {
            var notificacao = new Notificacao
            {
                UtilizadorId = inscricao.ApplicationUserId,
                Titulo = "Nova Inscrição Realizada ✅",
                Mensagem = $"Inscreveu-se com sucesso em {inscricao.Disciplina.Nome}.",
                Tipo = "Success",
                UrlController = "Perfil",
                UrlAction = "MinhasInscricoes"
            };

            await _notificacaoRepository.AddAsync(notificacao);
        }

        public async Task NotificarAtualizacaoDisciplinaAsync(Disciplina disciplina)
        {
            // Notificar todos os alunos inscritos nesta disciplina
            var inscricoes = await _inscricaoRepository.GetByDisciplinaIdAsync(disciplina.DisciplinaId);

            foreach (var inscricao in inscricoes.Where(i => i.Estado == "Ativa"))
            {
                var notificacao = new Notificacao
                {
                    UtilizadorId = inscricao.ApplicationUserId,
                    Titulo = "Disciplina Atualizada 📚",
                    Mensagem = $"A disciplina {disciplina.Nome} foi atualizada. Verifique as novas informações.",
                    Tipo = "Info",
                    UrlController = "Disciplinas",
                    UrlAction = "Details"
                    // ✅ REMOVIDA a duplicação - o ID vai na URL, não aqui
                };

                await _notificacaoRepository.AddAsync(notificacao);
            }
        }

        public async Task NotificarAproximacaoFimInscricaoAsync()
        {
            // Notificações automáticas (poderia ser chamado por um background service)
            _logger.LogInformation("Verificação de notificações automáticas executada.");
        }

        public async Task NotificarAchievementAsync(string utilizadorId, string achievement)
        {
            var notificacao = new Notificacao
            {
                UtilizadorId = utilizadorId,
                Titulo = "Achievement Desbloqueado! 🏆",
                Mensagem = achievement,
                Tipo = "Warning"
            };

            await _notificacaoRepository.AddAsync(notificacao);
        }
    }
}