using BookNest.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

       //Other Necessary DbSets
    }
}
