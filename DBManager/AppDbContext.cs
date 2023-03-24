using Models;
using Microsoft.EntityFrameworkCore;

namespace DBManager {
    public class AppDbContext : DbContext {
        #region Fields
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

        #endregion
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserLogin>().HasOne(c => c.User).WithOne(c => c.UserLogin).HasForeignKey<User>(fk => fk.Id);
            //modelBuilder.Entity<UserLogin>().HasIndex(u => u.Login).IsUnique();
            //modelBuilder.Entity<User>().HasIndex(u => u.Id).IsUnique();
            //modelBuilder.Entity<Role>().HasIndex(u => u.Name).IsUnique();
            //modelBuilder.Entity<Role>().HasMany(ua => ua.Users).WithOne(u => u.RoleUser).HasForeignKey(u => u.IdRole).OnDelete(DeleteBehavior.ClientSetNull);
            //modelBuilder.Entity<User>().HasMany(ua => ua.OrdersClients).WithOne(u => u.Client).HasForeignKey(u => u.IdClient).OnDelete(DeleteBehavior.ClientSetNull);
            //modelBuilder.Entity<User>().HasMany(ua => ua.OrdersMasters).WithOne(u => u.Master).HasForeignKey(u => u.IdMaster).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
