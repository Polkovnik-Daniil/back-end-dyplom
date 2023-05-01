using DBManager;
using DBManager.Pattern.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase {
        private readonly ILogger<RolesController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Role> _roleRepository;
        public RolesController(ILogger<RolesController> logger,
                               IServiceProvider serviceProvider) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _roleRepository = _unitOfWork.GetRepository<Role>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public async Task<IList<Role>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Roles : get request");
            return _roleRepository.GetPagedList(pageIndex: PageIndex).Items;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id) {
            _logger.LogInformation("/api/Roles : get Id request");
            var result = _roleRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Role value) {
            _logger.LogInformation("/api/Roles : post request");
            var IsExistNewValue = _roleRepository.Find(value.Id) is not null;
            if (!IsExistNewValue) {
                _roleRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Role value) {
            _logger.LogInformation("/api/Roles : put request");
            var oldValue = _roleRepository.Find(value.Id);
            if(oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _roleRepository.Update(value);
                _unitOfWork.SaveChanges();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");    
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            _logger.LogInformation("/api/Roles : delete request");
            var removedValue = _roleRepository.Find(id);
            if(removedValue is null) {
                return Ok("This value was deleted!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _roleRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
            return Ok("This value is deleted!");
        }
    }
}
