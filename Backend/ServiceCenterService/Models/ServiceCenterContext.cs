using Microsoft.EntityFrameworkCore;

namespace ServiceCenterService.Models
{
    public class ServiceCenterContext: DbContext
    {
        public ServiceCenterContext(DbContextOptions<ServiceCenterContext> options) : base(options)
        {
        }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }
    }
}
