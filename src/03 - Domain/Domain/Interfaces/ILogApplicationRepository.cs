using Domain.Models.LogApp;

namespace Domain.Interfaces
{
    public interface ILogApplicationRepository
    {
        Task InsertAsync(LogRequest log);
        IQueryable<LogRequest> GetLogs();
    }
}
