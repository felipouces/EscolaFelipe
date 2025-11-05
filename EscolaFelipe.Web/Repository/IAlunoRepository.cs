using EscolaFelipe.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EscolaFelipe.Web.Repository
{
    public interface IAlunoRepository
    {
        Task<IEnumerable<Aluno>> GetAllAsync();
        Task<Aluno> GetByIdAsync(int id);
        Task AddAsync(Aluno aluno);
        Task UpdateAsync(Aluno aluno);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}