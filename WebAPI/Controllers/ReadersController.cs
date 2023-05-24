using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebAPI.Filter;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [LocalAuthorization]
    public class ReadersController : ControllerBase {
        private readonly ILogger<ReadersController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Reader> _readerRepository;
        public ReadersController(ILogger<ReadersController> logger,
                                 IServiceProvider serviceProvider) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _readerRepository = _unitOfWork.GetRepository<Reader>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public async Task<IList<Reader>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Book : get request");
            return _readerRepository.GetPagedList(pageIndex: PageIndex,
                                                pageSize: 100).Items;
        }

        [HttpGet, Route("CountPage")]
        public int GetCountPage() {
            int count = _readerRepository.Count();
            return count % 20 == 0 ? (count / 20) : ((count / 20) + 1);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id = 0) {
            _logger.LogInformation("/api/Roles : get Id request");
            var result = _readerRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Post([FromBody] Reader value) {
            _logger.LogInformation("/api/Book : post request");
            var IsExistNewValue = _readerRepository.Find(value.Id) is not null;
            if (!IsExistNewValue) {
                _readerRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Put([FromBody] Reader value) {
            _logger.LogInformation("/api/Book : put request");
            var oldValue = _readerRepository.Find(value.Id);
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _readerRepository.Update(value);
                _unitOfWork.SaveChanges();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Delete(int id) {
            _logger.LogInformation("/api/Book : delete request");
            var removedValue = _readerRepository.Find(id);
            if (removedValue is null) {
                return BadRequest("This value is not exist!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _readerRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
            return Ok("This value is deleted!");
        }
    }
}
