using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Models.LogApp;

namespace Data.EntitesMaps.LogDataBase.LogApp
{
    internal class LogRequestMap : IEntityTypeConfiguration<LogRequest>
    {
        public void Configure(EntityTypeBuilder<LogRequest> builder)
        {
            builder.ToTable("LogRequests");
            builder.Property(l => l.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(l => l.TypeLog).HasColumnType("varchar(30)").IsRequired();
            builder.Property(l => l.UserName).HasColumnType("varchar(30)");
            builder.Property(l => l.Content).HasColumnType("varchar(150)");
            builder.Property(l => l.Method).HasColumnType("varchar(30)");
            builder.Property(l => l.Path).HasColumnType("varchar(100)");
            builder.Property(l => l.QueryString).HasColumnType("varchar(120)");
            builder.Property(l => l.InclusionDate).HasColumnType("datetime(6)").IsRequired();
            builder.Property(l => l.ExceptionMessage).HasColumnType("text");
            builder.Property(l => l.StackTrace).HasColumnType("text");

            builder.HasIndex(l => l.InclusionDate)
                .HasDatabaseName("IX_LogRequests_InclusionDate");

            builder.HasIndex(l => l.TypeLog)
                   .HasDatabaseName("IX_LogRequests_TypeLog");
        }
    }
}