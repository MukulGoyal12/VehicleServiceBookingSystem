using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace User_Management.Models
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) 
        { 
        }
        public DbSet<User> Users { get; set; }
    }
}


    
