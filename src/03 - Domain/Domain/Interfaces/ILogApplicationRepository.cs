using Domain.Models;

namespace Domain.Interfaces
{
    public interface ILogApplicationRepository
    {
        Task InsertAsync(LogRequest log);
        IQueryable<LogRequest> GetLogs();
    }
}
