using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers {
    [ApiController, 
     Authorize(Roles = "Admin"), 
     Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly ILogger<UsersController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public UsersController(ILogger<UsersController> logger,
                               IServiceProvider serviceProvider) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public async Task<IList<User>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Roles : get request");
            return _userRepository.GetPagedList(pageIndex: PageIndex,
                                                include: i => i.Include(x => x.Role)).Items;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id) {
            _logger.LogInformation("/api/Roles : get Id request");
            var result = _userRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User value) {
            _logger.LogInformation("/api/Roles : post request");
            var IsExistNewValue = _userRepository.Find(value.Id) is not null;
            if (!IsExistNewValue) {
                _userRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] User value) {
            _logger.LogInformation("/api/Roles : put request");
            var oldValue = _userRepository.Find(value.Id);
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _userRepository.Update(value);
                _unitOfWork.SaveChanges();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            _logger.LogInformation("/api/Roles : delete request");
            var removedValue = _userRepository.Find(id);
            if (removedValue is null) {
                return Ok("This value was deleted!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _userRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
            return Ok("This value is deleted!");
        }
    }
}
