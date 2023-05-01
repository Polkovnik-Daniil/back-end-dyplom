using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly ILogger<BooksController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Book> _bookRepository;
        public BooksController(ILogger<BooksController> logger,
                              IServiceProvider serviceProvider,
                              ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookRepository = _unitOfWork.GetRepository<Book>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public async Task<IList<Book>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Book : get request");
            int count = _bookRepository.Count();
            return _bookRepository.GetPagedList(pageIndex: PageIndex,
                                                include: i => i.Include(x => x.Genres)).Items;
        }
        [HttpGet, Route("CountPage")]
        public async Task<int> GetCountPage() {
            int count = _bookRepository.Count();
            return count % 20 == 0 ? (count / 20) : ((count / 20) + 1);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id = 0) {
            _logger.LogInformation("/api/Roles : get Id request");
            var result = _bookRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Post([FromBody] Book value) {
            _logger.LogInformation("/api/Book : post request");
            var IsExistNewValue = _bookRepository.Find(value.Id) is not null;
            if (!IsExistNewValue) {
                _bookRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Put([FromBody] Book value) {
            _logger.LogInformation("/api/Book : put request");
            var oldValue = _bookRepository.Find(value.Id);
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _bookRepository.Update(value);
                _unitOfWork.SaveChanges();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin, Moderator")]
        public IActionResult Delete(int id) {
            _logger.LogInformation("/api/Book : delete request");
            var removedValue = _bookRepository.Find(id);
            if (removedValue is null) {
                return BadRequest("This value is not exist!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _bookRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
            return Ok("This value is deleted!");
        }
    }
}
