using Data.DataContext;
using Domain.Interfaces;
using Domain.Models.LogApp;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.LogApp
{
    public class LogApplicationRepository(LogDbContext LogDbContext) : ILogApplicationRepository
    {
        private DbSet<LogRequest> DbSet { get; } = LogDbContext.LogsRequests;
        public async Task InsertAsync(LogRequest log)
        {
            await DbSet.AddAsync(log);
            await LogDbContext.SaveChangesAsync();
        }

        public IQueryable<LogRequest> GetLogs() => DbSet;
    }
}
