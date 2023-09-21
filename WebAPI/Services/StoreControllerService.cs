using DBManager.Pattern.Interface;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Services
{
    public class StoreControllerService<Entity> where Entity : class
    {
        private IRepository<Entity> _repository;
        public StoreControllerService(IRepository<Entity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<IPagedList<Entity>> GetPage(int PageIndex = 0)
        {
            try
            {

                return null;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
