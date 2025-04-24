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
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Badge> Badges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BooksGenre"));

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Badges)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BooksBadge"));
        }


    }
}
