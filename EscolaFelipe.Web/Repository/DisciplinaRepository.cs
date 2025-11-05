using EscolaFelipe.Web.Data;
using EscolaFelipe.Web.Models;
using EscolaFelipeWeb.Repository;
using Microsoft.EntityFrameworkCore;

namespace EscolaFelipeTeste.Repository
{
    public class DisciplinaRepository : IDisciplinaRepository
    {
        private readonly ApplicationDbContext _context;

        public DisciplinaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Disciplina>> GetAllAsync()
        {
            return await _context.Disciplinas.ToListAsync();
        }

        public async Task<IEnumerable<Disciplina>> GetAtivasAsync()
        {
            return await _context.Disciplinas.Where(d => d.Ativo).ToListAsync();
        }

        public async Task<Disciplina> GetByIdAsync(int id)
        {
            return await _context.Disciplinas.FindAsync(id);
        }

        public async Task AddAsync(Disciplina disciplina)
        {
            try
            {
                await _context.Disciplinas.AddAsync(disciplina);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao adicionar disciplina.", ex);
            }
        }

        public async Task UpdateAsync(Disciplina disciplina)
        {
            try
            {
                _context.Disciplinas.Update(disciplina);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao atualizar disciplina.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var disciplina = await GetByIdAsync(id);
                if (disciplina != null)
                {
                    _context.Disciplinas.Remove(disciplina);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao eliminar disciplina. Pode ter inscrições associadas.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Disciplinas.AnyAsync(d => d.DisciplinaId == id);
        }
    }
}