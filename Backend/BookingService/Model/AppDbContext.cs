using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookingService.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BookingId to start from 100
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingId)
                .UseIdentityColumn(100, 1);
        }
    }
}
