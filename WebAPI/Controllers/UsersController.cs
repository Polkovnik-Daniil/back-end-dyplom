using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Authorization;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [ApiController,
     Authorize(Roles = "Admin"),
     Route("api/[controller]")]
    [LocalAuthorization]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public UsersController(ILogger<UsersController> logger,
                               IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("UsersController", "NLog injected into UsersController");
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_userRepository.GetPagedList(pageIndex: PageIndex,
                                                    include: i => i.Include(x => x.Role),
                                                    pageSize: 100).Items);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                _logger.LogInformation("GET REQUEST ID");
                var result = _userRepository.Find(id);
                if(result is null)
                {
                    return BadRequest("Value is not exist!");
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertElement([FromBody] User value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var IsExistNewValue = _userRepository.GetFirstOrDefault(predicate: x=> x.Email == value.Email) is not null;
                if(!IsExistNewValue)
                {
                    _userRepository.Insert(value);
                    _unitOfWork.SaveChanges();
                    return Ok("This value was added!");
                }
                return StatusCode(420);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateElement([FromBody] User value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");
                var oldValue = _userRepository.Find(value.Id);
                if(oldValue is null)
                {
                    StatusCode(420);
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _userRepository.Update(value);
                    _unitOfWork.SaveChanges();
                    return Ok("This value is update!");
                }
                return Ok("This value is actually");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElement(int id)
        {
            try
            {
                _logger.LogInformation("DELETE REQUEST");
                var removedValue = _userRepository.Find(id);
                if(removedValue is null)
                {
                    return Ok("This value was deleted!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _userRepository.Delete(removedValue!);
                _unitOfWork.SaveChanges();
                return Ok("This value is deleted!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
