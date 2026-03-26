using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext>options):DbContext(options)
    {
            public DbSet<User> Users { get; set; }

    }
}
