using System.Linq.Expressions;

namespace API_Background.Repositories
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> AdicionarAsync(TEntity entidade);
        Task<TEntity> AtualizarAsync(TEntity entidade);
        Task<IEnumerable<TEntity>> AtualizarEmLoteAsync(IEnumerable<TEntity> entidade);
        Task<TEntity?> ObterAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> ObterTodosAsync();
        Task<IEnumerable<TEntity>> ObterTodosAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
