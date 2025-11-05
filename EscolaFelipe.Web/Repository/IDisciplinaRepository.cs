using EscolaFelipe.Web.Models;


namespace EscolaFelipeWeb.Repository
{
    public interface IDisciplinaRepository
    {
        Task<IEnumerable<Disciplina>> GetAllAsync();
        Task<IEnumerable<Disciplina>> GetAtivasAsync();
        Task<Disciplina> GetByIdAsync(int id);
        Task AddAsync(Disciplina disciplina);
        Task UpdateAsync(Disciplina disciplina);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}