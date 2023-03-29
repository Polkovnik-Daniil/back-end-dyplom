using DBManager;
using DBManager.Pattern.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase {
        private readonly ILogger<RolesController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Role> _roleRepository;
        public RolesController(ILogger<RolesController> logger, IServiceProvider serviceProvider) {
            _logger = logger;
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>();
            _roleRepository = _unitOfWork.GetRepository<Role>();
        }

        [HttpGet]
        public IQueryable<Role> Get() {
            _logger.LogInformation("/api/Roles : get request");
            var result = _roleRepository.GetAll(false);
            return result;
        }

        [HttpGet("{id}")]
        public Role? Get(int id) {
            _logger.LogInformation("/api/Roles : get Id request");
            return _roleRepository.Find(id);
        }

        [HttpPost]
        public void Post([FromBody] Role value) {
            _logger.LogInformation("/api/Roles : post request");
            _roleRepository.Insert(value);
            _unitOfWork.SaveChanges();
        }

        [HttpPut]
        public void Put([FromBody] Role value) {
            _logger.LogInformation("/api/Roles : put request");
            var oldValue = _roleRepository.Find(value.Id);
            _unitOfWork.DbContext.Entry(oldValue).State = EntityState.Detached;
            bool IsEqualOldValue = oldValue.Equals(value);
            if (!IsEqualOldValue) {
                //_unitOfWork.DbContext.ChangeTracker.Clear();
                //oldValue.Name = value.Name;
                _roleRepository.Update(value);
                _unitOfWork.SaveChanges();
                //_unitOfWork.DbContext.Entry(value).State = EntityState.Modified;
            }
        }

        [HttpDelete("{id}")]
        public void Delete(int id) {
            _logger.LogInformation("/api/Roles : delete request");
            _roleRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }
    }
}
