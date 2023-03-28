namespace DBManager.Pattern.Inteface {
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetList();
        T GetItem(int id);
        void Create(T item);
        void Update(T item);
        void Delete(T item);
    }
}
