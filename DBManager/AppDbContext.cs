using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DBManager {
    public class AppDbContext : DbContext {
        #region Fields
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Reader> Readers { get; set; }

        public DbSet<BookReader> BookReaders { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }



        #endregion
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Role>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<Genre>().HasIndex(u => u.Name).IsUnique();

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

            //Первые инициализируемые значения
            modelBuilder.Entity<Role>().HasData(
                new Role[] {
                    new Role { Id = 1, Name = "Admin"       },
                    new Role { Id = 2, Name = "Moderator"   },
                    new Role { Id = 3, Name = "User"        }
                });


            modelBuilder.Entity<User>().HasData(
                new User[] {
                    new User { Id = 1, Email = "admin@example.com",     Name = "admin",      Password = "string", IsLocked = false, RoleId = 1 },
                    new User { Id = 2, Email = "moderator@example.com", Name = "modeartor",  Password = "string", IsLocked = false, RoleId = 2 },
                    new User { Id = 3, Email = "user@example.com",      Name = "user",       Password = "string", IsLocked = false, RoleId = 3 }
                });
        }
    }
}
