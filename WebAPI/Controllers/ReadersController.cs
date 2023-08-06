using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LocalAuthorization]
    public class ReadersController : ControllerBase
    {
        private readonly ILogger<ReadersController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Reader> _readerRepository;
        public ReadersController(ILogger<ReadersController> logger,
                                 IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _readerRepository = _unitOfWork.GetRepository<Reader>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("ReadersController", "NLog injected into ReadersController");
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_readerRepository.GetPagedList(pageIndex: PageIndex,
                                                    pageSize: 100).Items);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet, Route("CountPage")]
        public async Task<IActionResult> GetCountPage()
        {
            try
            {
                int count = _readerRepository.CountAsync().Result;
                return Ok(count % 20 == 0 ? (count / 20) : ((count / 20) + 1));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST ID");
                var result = _readerRepository.Find(id);
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

        [HttpPost, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> InsertElement([FromBody] Reader value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var IsExistNewValue = _readerRepository.Find(value.Id) is not null;
                if(!IsExistNewValue)
                {
                    _readerRepository.Insert(value);
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

        [HttpPut, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> UpdateElement([FromBody] Reader value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");
                var oldValue = _readerRepository.Find(value.Id);
                if(oldValue is null)
                {
                    BadRequest("Values is not exist!");
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _readerRepository.Update(value);
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

        [HttpDelete("{id}"), Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteElement(int id)
        {
            try
            {
                _logger.LogInformation("DELETE REQUEST");
                var removedValue = _readerRepository.Find(id);
                if(removedValue is null)
                {
                    return BadRequest("This value is not exist!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _readerRepository.Delete(removedValue!);
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
