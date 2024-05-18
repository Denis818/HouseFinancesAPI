using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories.Base
{
    public interface IRepositoryBase<TEntity>
        where TEntity : class, new()
    {
        void Delete(TEntity entity);
        void Update(TEntity entity);
        Task<bool> SaveChangesAsync();
        Task InsertAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        void DeleteRange(TEntity[] entityArray);
        Task InsertRangeAsync(List<TEntity> entity);
        IQueryable<TEntity> ExecuteRawSqlQuery(string query);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null);
    }
}
