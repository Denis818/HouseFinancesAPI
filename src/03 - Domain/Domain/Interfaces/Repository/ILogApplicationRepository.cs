using Domain.Models;

namespace Domain.Interfaces.Repository
{
    public interface ILogApplicationRepository
    {
        Task InsertAsync(LogApplication log);
        IQueryable<LogApplication> GetLogs();
    }
}
