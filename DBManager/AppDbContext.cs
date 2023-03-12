using Microsoft.EntityFrameworkCore;

namespace DBManager {
    public class AppDbContext : DbContext {
        #region Fields
        //public DbSet<..> .. { get; set; }
        #endregion
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            Database.EnsureCreated();
        }
    }
}
