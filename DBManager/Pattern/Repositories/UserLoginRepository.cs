using Models;
using DBManager.Pattern.Inteface;
using Microsoft.EntityFrameworkCore;

namespace DBManager.Pattern.Repositories {
    public class UserLoginRepository : IRepository<UserLogin> {
        private AppDbContext db;
        public UserLoginRepository(AppDbContext context) {
            db = context;
        }
        public void Create(UserLogin item) {
            db.UserLogins.Add(item);
        }
        public void Delete(UserLogin item) {
            UserLogin user = db.UserLogins.Find(item.Login);
            if (user != null) {
                db.UserLogins.Remove(item);
            }
        }
        public UserLogin GetItem(int id) {
            return db.UserLogins.Find(id);
        }
        public IEnumerable<UserLogin> GetList() {
            return db.UserLogins;
        }
        public void Save() {
            db.SaveChanges();
        }
        public void Update(UserLogin item) {
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
