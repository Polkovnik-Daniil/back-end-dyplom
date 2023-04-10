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
    public class BookController : ControllerBase {
        private readonly ILogger<BookController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Book> _bookRepository;
        public BookController(ILogger<BookController> logger,
                              IServiceProvider serviceProvider,
                              ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookRepository = _unitOfWork.GetRepository<Book>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public IList<Book> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Book : get request");
            return _bookRepository.GetPagedList(pageIndex: PageIndex).Items;
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id = 0) {
            _logger.LogInformation("/api/Roles : get Id request");
            var result = _bookRepository.Find(id);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Post([FromBody] Book value) {
            _logger.LogInformation("/api/Book : post request");
            var IsExistNewValue = _bookRepository.Find(value.Id) is not null;
            if (!IsExistNewValue) {
                _bookRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Put([FromBody] Book value) {
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Moderator")]
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
