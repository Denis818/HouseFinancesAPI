using Domain.Models.LogApp;

namespace Domain.Interfaces.Repository.LogApp
{
    public interface ILogApplicationRepository
    {
        Task InsertAsync(LogApplication log);
        IQueryable<LogApplication> GetLogs();
    }
}
