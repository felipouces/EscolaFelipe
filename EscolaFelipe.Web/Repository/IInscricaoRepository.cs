using EscolaFelipe.Web.Models;

namespace EscolaFelipe.Web.Repository
{
    public interface IInscricaoRepository
    {
        Task<IEnumerable<Inscricao>> GetAllAsync();
        Task<Inscricao> GetByIdAsync(int id);
        Task<IEnumerable<Inscricao>> GetByAlunoIdAsync(int alunoId);
        Task<IEnumerable<Inscricao>> GetByDisciplinaIdAsync(int disciplinaId);
        Task AddAsync(Inscricao inscricao);
        Task UpdateAsync(Inscricao inscricao);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> AlunoInscritoEmDisciplinaAsync(int alunoId, int disciplinaId);
    }
}