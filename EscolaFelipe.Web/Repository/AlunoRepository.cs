using EscolaFelipe.Web.Data;
using EscolaFelipe.Web.Repository;
using EscolaFelipe.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EscolaFelipe.Web.Repository
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly ApplicationDbContext _context;

        public AlunoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Aluno>> GetAllAsync()
        {
            return await _context.Alunos.ToListAsync();
        }

        public async Task<Aluno> GetByIdAsync(int id)
        {
            return await _context.Alunos.FindAsync(id);
        }

        public async Task AddAsync(Aluno aluno)
        {
            try
            {
                await _context.Alunos.AddAsync(aluno);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao adicionar aluno. Verifique os dados e tente novamente.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado ao adicionar aluno.", ex);
            }
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            try
            {
                _context.Alunos.Update(aluno);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex) // Esta deve vir PRIMEIRO
            {
                throw new Exception("O aluno foi modificado por outro utilizador. Recarregue os dados e tente novamente.", ex);
            }
            catch (DbUpdateException ex) // Esta deve vir DEPOIS
            {
                throw new Exception("Erro ao atualizar aluno. Verifique os dados e tente novamente.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado ao atualizar aluno.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var aluno = await GetByIdAsync(id);
                if (aluno != null)
                {
                    _context.Alunos.Remove(aluno);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Erro ao eliminar aluno. O aluno pode estar associado a outras entidades.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado ao eliminar aluno.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Alunos.AnyAsync(a => a.AlunoId == id);
        }
    }
}