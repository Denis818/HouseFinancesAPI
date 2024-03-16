using Data.DataContext;
using Domain.Interfaces.Repository.LogApp;
using Domain.Models.LogApp;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.LogApp
{
    public class LogApplicationRepository(LogDbContext LogDbContext) : ILogApplicationRepository
    {
        private DbSet<LogApplication> DbSet { get; } = LogDbContext.LogsApplication;
        public async Task InsertAsync(LogApplication log)
        {
            await DbSet.AddAsync(log);
            await LogDbContext.SaveChangesAsync();
        }

        public IQueryable<LogApplication> GetLogs() => DbSet;
    }
}
