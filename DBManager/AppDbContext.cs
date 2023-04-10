using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DBManager {
    public class AppDbContext : DbContext {
        #region Fields
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Reader> Readers { get; set; }


        #endregion
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Role>().HasIndex(u => u.Name).IsUnique();

            modelBuilder
                .Entity<Book>()
                .HasMany(c => c.Authors)
                .WithMany(s => s.Books)
                .UsingEntity<BookAuthor>(
                   j => j
                    .HasOne(pt => pt.Author)
                    .WithMany(t => t.BookAuthors)
                    .HasForeignKey(pt => pt.AuthorId),
                j => j
                    .HasOne(pt => pt.Book)
                    .WithMany(p => p.BookAuthors)
                    .HasForeignKey(pt => pt.BookId),
                j => {
                    j.HasKey(t => new { t.AuthorId, t.BookId });
                    j.ToTable("BookAuthor");
                });

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Genres)
                .WithMany(s => s.Books)
                .UsingEntity<BookGenre>(
                   j => j
                    .HasOne(pt => pt.Genre)
                    .WithMany(t => t.BookGenre)
                    .HasForeignKey(pt => pt.GenreId),
                j => j
                    .HasOne(pt => pt.Book)
                    .WithMany(p => p.BookGenre)
                    .HasForeignKey(pt => pt.BookId),
                j => {
                    j.HasKey(t => new { t.GenreId, t.BookId });
                    j.ToTable("BookGenre");
                });

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Readers)
                .WithMany(s => s.Books)
                .UsingEntity<BookReader>(
                   j => j
                    .HasOne(pt => pt.Reader)
                    .WithMany(t => t.BookReader)
                    .HasForeignKey(pt => pt.ReaderId),
                j => j
                    .HasOne(pt => pt.Book)
                    .WithMany(p => p.BookReader)
                    .HasForeignKey(pt => pt.BookId),
                j => {
                    j.HasKey(t => new { t.ReaderId, t.BookId });
                    j.ToTable("BookReader");
                });

            //modelBuilder.Entity<Role>().HasMany(ua => ua.Users)
            //                           .WithOne(u => u.RoleUser)
            //                           .HasForeignKey(u => u.IdRole)
            //                           .OnDelete(DeleteBehavior.ClientSetNull);
            //modelBuilder.Entity<User>().HasMany(ua => ua.OrdersClients).WithOne(u => u.Client).HasForeignKey(u => u.IdClient).OnDelete(DeleteBehavior.ClientSetNull);
            //modelBuilder.Entity<User>().HasMany(ua => ua.OrdersMasters).WithOne(u => u.Master).HasForeignKey(u => u.IdMaster).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
