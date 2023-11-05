using DataModels.EFModels.QueueEntities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.QueueRepo
{
    public class QueueDbContext : DbContext
    {
        public QueueDbContext(DbContextOptions<QueueDbContext> options)
           : base(options) { }

        public DbSet<EventQueue> EventQueue { get; set; }

        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>().HasData(
                new Settings { Key = "Cursor", Value = "0" }
            );
        }
    }
}
