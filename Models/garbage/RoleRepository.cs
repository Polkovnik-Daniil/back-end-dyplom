using Models;
using DBManager.Pattern.Inteface;
using Microsoft.EntityFrameworkCore;

namespace DBManager.Pattern.Repositories {
    public class RoleRepository : IRepository<Role> {
        private AppDbContext db;
        public RoleRepository(AppDbContext context) {
            db = context;
        }
        public void Create(Role item) {
            db.Roles.Add(item);
        }
        public void Delete(Role item) {
            Role user = db.Roles.Find(item.Id);
            if (user != null) {
                db.Roles.Remove(item);
            }
        }
        public Role GetItem(int id) {
            return db.Roles.Find(id);
        }
        public IEnumerable<Role> GetList() {
            return db.Roles;
        }
        public void Save() {
            db.SaveChanges();
        }
        public void Update(Role item) {
            db.Entry(item).State = EntityState.Modified;
        }

        private bool disposed = false;
            
        public virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
