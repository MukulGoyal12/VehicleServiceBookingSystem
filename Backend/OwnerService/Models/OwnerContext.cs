using Microsoft.EntityFrameworkCore;

namespace OwnerService.Models
{
    public class OwnerContext:DbContext
    {
        public OwnerContext(DbContextOptions<OwnerContext> options) : base(options)
        {
        }
        public DbSet<Owner> Owners { get; set; }
    }
}
