using Models;
using DBManager.Pattern.Inteface;
using Microsoft.EntityFrameworkCore;

namespace DBManager.Pattern.Repositories {
    public class UserRepository : IRepository<User> {
        private AppDbContext db;
        public UserRepository(AppDbContext context) {
            db = context;
        }
        public void Create(User item) {
            db.Users.Add(item);
        }
        public void Delete(User item) {
            User user = db.Users.Find(item.Id);
            if(user != null) {
                db.Users.Remove(item);
            }
        }
        public User GetItem(int id) {
            return db.Users.Find(id);
        }
        public IEnumerable<User> GetList() {
            return db.Users;
        }
        public void Save() {
            db.SaveChanges();
        }
        public void Update(User item) {
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

