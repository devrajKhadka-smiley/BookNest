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
        //Other Necessary DbSets
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }


        // Whitelist table
        public DbSet<Whitelist> Whitelists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Book>()
            //    .HasOne(b => b.Author)
            //    .WithMany()
            //    .HasForeignKey(b => b.BookAuthorId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Author)
                .WithMany(a => a.Books)
                .UsingEntity(j => j.ToTable("BooksAuthor"));

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publication)
                .WithMany()
                .HasForeignKey(b => b.BookPublicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BooksGenre"));

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Badges)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BooksBadge"));

            //-------Cart content
            modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique();

            modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany()
                .HasForeignKey(ci => ci.BookId);

            //Whitelist
            modelBuilder.Entity<Whitelist>()
                .HasOne(w => w.Book)
                .WithMany()
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
