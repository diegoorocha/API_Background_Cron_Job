using API_Background.Data;
using API_Background.Exceptions;
using API_Background.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_Background.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly BackgroundDbContext _backgroundDbContext;
        protected readonly DbSet<TEntity> _DbSet;

        public BaseRepository(BackgroundDbContext backgroundDbContext)
        {
            _backgroundDbContext = backgroundDbContext;
            _DbSet = _backgroundDbContext.Set<TEntity>();
        }

        public virtual async Task<TEntity> AdicionarAsync(TEntity entidade)
        {
            try
            {
                _DbSet.Add(entidade);

                await Salvar(entidade);

                return entidade;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao salvar a entidade {typeof(TEntity).Name}.", ex);
            }
        }
        public virtual async Task<TEntity> AtualizarAsync(TEntity entidade)
        {
            try
            {
                _backgroundDbContext.Update(entidade);

                await Salvar(entidade);

                return entidade;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao atualizar a entidade: {typeof(TEntity).Name}.", ex);
            }
        }
        public virtual async Task<IEnumerable<TEntity>> AtualizarEmLoteAsync(IEnumerable<TEntity> entidade)
        {
            try
            {
                _backgroundDbContext.UpdateRange(entidade);

                await Salvar(entidade);

                return entidade;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao atualizar a entidade: {typeof(TEntity).Name}.", ex);
            }
        }
        public virtual async Task<TEntity?> ObterAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                IQueryable<TEntity> query = _DbSet;

                // Chama o método para incluir os relacionamentos
                query = IncluirRelacionamentos(query);

                return await query.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao obter a condição: {typeof(TEntity).Name}", ex);
            }
        }
        public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync()
        {
            try
            {
                IQueryable<TEntity> query = _DbSet;

                // Chama o método para incluir os relacionamentos
                query = IncluirRelacionamentos(query);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao obter todos.", ex);
            }
        }
        public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                IQueryable<TEntity> query = _DbSet;

                // Chama o método para incluir os relacionamentos
                query = IncluirRelacionamentos(query);

                return await query.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao obter a condição: {typeof(TEntity).Name}", ex);
            }
        }
        public void Dispose()
        {
            _backgroundDbContext?.Dispose();
        }
        private async Task<bool> Salvar(TEntity entidade)
        {
            try
            {
                return await _backgroundDbContext.SaveChangesAsync() == 1;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao salvar no banco de dados a entidade {entidade}.", ex);
            }
        }
        private async Task<bool> Salvar(IEnumerable<TEntity> entidade)
        {
            try
            {
                return await _backgroundDbContext.SaveChangesAsync() == 1;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Ocorreu um erro ao salvar em lote no banco de dados a entidade {entidade}.", ex);
            }
        }

        // Método para incluir relacionamentos baseados no tipo da entidade
        private static IQueryable<TEntity> IncluirRelacionamentos(IQueryable<TEntity> query)
        {
            #region Entidade Venda

            // Verifica se o tipo TEntity é Venda e inclui o Cliente
            if (typeof(TEntity) == typeof(Venda))
            {
                query = query
                    .Include("Cliente") // Inclui o relacionamento com Cliente
                    .Include("CarrosVenda")  // Inclui a coleção CarrosVenda
                    .Include("CarrosVenda.Carro") // Inclui o relacionamento Carro dentro de CarrosVenda
                    .Include("LogsProcessamento"); // Inclui a coleção LogsProcessamento
            }

            #endregion Entidade Venda


            return query;
        }

    }
}
