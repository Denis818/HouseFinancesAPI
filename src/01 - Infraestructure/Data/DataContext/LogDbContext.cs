using Data.EntitesMaps.LogDataBase.LogApp;
using Domain.Models.LogApp;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext
{
    public class LogDbContext(DbContextOptions<LogDbContext> options) : DbContext(options)
    {
        public DbSet<LogRequest> LogsRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new LogRequestMap());
        }
    }
}