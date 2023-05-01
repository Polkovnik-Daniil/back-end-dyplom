using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace WebAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/History")]
    public class BookReadersController : ControllerBase {
        private readonly ILogger<AuthorsController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<BookReader> _bookReadersRepository;
        public BookReadersController(ILogger<AuthorsController> logger,
                                     IServiceProvider serviceProvider) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookReadersRepository = _unitOfWork.GetRepository<BookReader>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }
        [HttpGet]
        public async Task<IList<BookReader>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/History : get request");
            return _bookReadersRepository.GetPagedList(pageIndex: PageIndex,
                                                       include: i => i.Include(x => x.Book)
                                                                      .Include(x => x.Reader)).Items;
        }
        [HttpGet, Route("CountPage")]
        public async Task<int> GetCountPage() {
            int count = _bookReadersRepository.Count();
            return count % 20 == 0 ? (count / 20) : ((count / 20) + 1);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id) {
            _logger.LogInformation("/api/History : get Id request");
            var result = _bookReadersRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookReader value) {
            _logger.LogInformation("/api/History : post request");
            var newValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x.Equals(value) && x.DateTimeEnd == value.DateTimeEnd && x.DateTimeStart == value.DateTimeStart);
            var IsExistNewValue = newValue is not null;
            if (!IsExistNewValue) {
                _bookReadersRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] BookReader value) {
            _logger.LogInformation("/api/History : put request");
            var oldValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x == value);
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _bookReadersRepository.Update(value);
                await _unitOfWork.SaveChangesAsync();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");
        }
        [HttpDelete("{readerId:int}/{bookId:int}/{start:datetime}/{end:datetime}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Delete(int readerId, int bookId, DateTime start, DateTime end) {
            _logger.LogInformation("/api/History : delete request");
            var removedValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x.BookId == bookId 
                                                                                    && x.ReaderId == readerId 
                                                                                    && x.DateTimeStart == start 
                                                                                    && x.DateTimeEnd == end);
            if (removedValue is null) {
                return Ok("This value was deleted!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _bookReadersRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
            return Ok("This value is deleted!");
        }
    }
}
