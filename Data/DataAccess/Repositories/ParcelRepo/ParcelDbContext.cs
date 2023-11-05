using DataModels.EFModels.ParcelEntities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.ParcelRepo
{
    public class ParcelDbContext : DbContext
    {
        public ParcelDbContext(DbContextOptions<ParcelDbContext> options)
           : base(options) { }

        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parcel>().Property(e => e.ParcelId).ValueGeneratedNever();
            modelBuilder.Entity<Event>().Property(e => e.EventId).ValueGeneratedNever();
            modelBuilder.Entity<Event>().Property(e => e.ParcelId).ValueGeneratedNever();
        }
    }
}
