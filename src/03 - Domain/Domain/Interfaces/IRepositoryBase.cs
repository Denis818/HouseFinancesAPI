using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IRepositoryBase<TEntity>
        where TEntity : class, new()
    {
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null);
        Task<TEntity> GetByIdAsync(int id);
        Task InsertAsync(TEntity entity);
        Task InsertRangeAsync(List<TEntity> entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void DeleteRange(TEntity[] entity);
        Task<bool> SaveChangesAsync();
    }
}
