namespace DBManager.Pattern {
    public class UnitOfWork {
        private AppDbContext ApplicationDbСontext;
        public UnitOfWork(AppDbContext appDbContext) {
            ApplicationDbСontext = appDbContext;
        }
    }
}
