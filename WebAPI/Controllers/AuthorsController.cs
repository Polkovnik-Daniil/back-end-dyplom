using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.AspNetCore.Authorization;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [LocalAuthorization]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Author> _authorRepository;
        public AuthorsController(ILogger<AuthorsController> logger,
                                 IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _authorRepository = _unitOfWork.GetRepository<Author>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("AuthorsController", "NLog injected into AuthorsController");
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_authorRepository.GetPagedList(pageIndex: PageIndex,
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
                var result = _authorRepository.Find(id);
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
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> InsertElement([FromBody] Author value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var IsExistNewValue = _authorRepository.Find(value.Id) is not null;
                if(!IsExistNewValue)
                {
                    _authorRepository.Insert(value);
                    _unitOfWork.SaveChanges();
                    return Ok("This value was added!");
                }
                return Ok("This value is exist!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> UpdateElement([FromBody] Author value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");
                var oldValue = _authorRepository.Find(value.Id);
                if(oldValue is null)
                {
                    BadRequest("Values is not exist!");
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _authorRepository.Update(value);
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
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteElemenet(int id)
        {
            try
            {
                _logger.LogInformation("DELETE REQUEST");
                var removedValue = _authorRepository.Find(id);
                if(removedValue is null)
                {
                    return Ok("This value was deleted!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _authorRepository.Delete(removedValue!);
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
