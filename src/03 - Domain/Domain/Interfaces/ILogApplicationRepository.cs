using Domain.Models;

namespace Domain.Interfaces
{
    public interface ILogApplicationRepository
    {
        Task InsertAsync(LogApplication log);
        IQueryable<LogApplication> GetLogs();
    }
}
