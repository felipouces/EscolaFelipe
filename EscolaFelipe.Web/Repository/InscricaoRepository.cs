using EscolaFelipe.Web.Data;
using EscolaFelipe.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaFelipe.Web.Repository
{
    public class InscricaoRepository : IInscricaoRepository
    {
        private readonly ApplicationDbContext _context;

        public InscricaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inscricao>> GetAllAsync()
        {
            return await _context.Inscricoes
                .Include(i => i.Aluno)
                .Include(i => i.Disciplina)
                .Include(i => i.ApplicationUser)
                .ToListAsync();
        }

        public async Task<Inscricao> GetByIdAsync(int id)
        {
            return await _context.Inscricoes
                .Include(i => i.Aluno)
                .Include(i => i.Disciplina)
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(i => i.InscricaoId == id);
        }

        public async Task<IEnumerable<Inscricao>> GetByAlunoIdAsync(int alunoId)
        {
            return await _context.Inscricoes
                .Include(i => i.Aluno)
                .Include(i => i.Disciplina)
                .Include(i => i.ApplicationUser)
                .Where(i => i.AlunoId == alunoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscricao>> GetByDisciplinaIdAsync(int disciplinaId)
        {
            return await _context.Inscricoes
                .Include(i => i.Aluno)
                .Include(i => i.Disciplina)
                .Include(i => i.ApplicationUser)
                .Where(i => i.DisciplinaId == disciplinaId)
                .ToListAsync();
        }

        public async Task AddAsync(Inscricao inscricao)
        {
            try
            {
                await _context.Inscricoes.AddAsync(inscricao);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao adicionar inscrição.", ex);
            }
        }

        public async Task UpdateAsync(Inscricao inscricao)
        {
            try
            {
                _context.Inscricoes.Update(inscricao);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao atualizar inscrição.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var inscricao = await GetByIdAsync(id);
                if (inscricao != null)
                {
                    _context.Inscricoes.Remove(inscricao);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao eliminar inscrição.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Inscricoes.AnyAsync(i => i.InscricaoId == id);
        }

        public async Task<bool> AlunoInscritoEmDisciplinaAsync(int alunoId, int disciplinaId)
        {
            return await _context.Inscricoes
                .AnyAsync(i => i.AlunoId == alunoId && i.DisciplinaId == disciplinaId && i.Estado == "Ativa");
        }
    }
}