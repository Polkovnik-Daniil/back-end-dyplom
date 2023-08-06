using DBManager;
using DBManager.Pattern.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebAPI.Filter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [LocalAuthorization]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Role> _roleRepository;
        public RolesController(ILogger<RolesController> logger,
                               IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _roleRepository = _unitOfWork.GetRepository<Role>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("RolesController", "NLog injected into RolesController");
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_roleRepository.GetPagedList(pageIndex: PageIndex,
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
                var result = _roleRepository.Find(id);
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
        public async Task<IActionResult> InsertElement([FromBody] Role value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var IsExistNewValue = _roleRepository.Find(value.Id) is not null;
                if(!IsExistNewValue)
                {
                    _roleRepository.Insert(value);
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
        public async Task<IActionResult> UpdateElement([FromBody] Role value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");
                var oldValue = _roleRepository.Find(value.Id);
                if(oldValue is null)
                {
                    BadRequest("Values is not exist!");
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _roleRepository.Update(value);
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
                var removedValue = _roleRepository.Find(id);
                if(removedValue is null)
                {
                    return Ok("This value was deleted!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _roleRepository.Delete(removedValue!);
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
