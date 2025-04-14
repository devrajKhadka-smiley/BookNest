using BookNest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users {  get; set; }
        public DbSet<Badges> Badges {  get; set; }
    }
}
