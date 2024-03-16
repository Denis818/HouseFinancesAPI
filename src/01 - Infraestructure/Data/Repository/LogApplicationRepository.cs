using Data.DataContext;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
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
