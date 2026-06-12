using Microsoft.EntityFrameworkCore;

namespace VehicleService.Models
{
    public class VehicleContext:DbContext
    {
        public VehicleContext(DbContextOptions<VehicleContext> options) : base(options)
        {

        }
        public DbSet<Vehicle> Vehicles { get; set; }
    }

}
