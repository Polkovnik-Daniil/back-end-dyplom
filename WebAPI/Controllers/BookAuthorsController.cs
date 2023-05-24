using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace WebAPI.Controllers {
    [Route("api/Authorship")]
    [ApiController]
    public class BookAuthorsController : ControllerBase {
        private readonly ILogger<BookAuthorsController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<BookAuthor> _bookAuthorRepository;
        public BookAuthorsController(ILogger<BookAuthorsController> logger,
                                     IServiceProvider serviceProvider) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookAuthorRepository = _unitOfWork.GetRepository<BookAuthor>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
        }

        [HttpGet]
        public async Task<IList<BookAuthor>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Authorship : get request");
            return _bookAuthorRepository.GetPagedList(pageIndex: PageIndex,
                                                      include: i => i.Include(x => x.Book)
                                                                     .Include(x => x.Author),
                                                      pageSize: 1000).Items;
        }

        [HttpGet("{authorId:int}/{bookId:int}")]
        public async Task<IActionResult> GetItem(int authorId, int bookId) {
            _logger.LogInformation("/api/Authorship : get Id request");
            var result = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == authorId && x.BookId == bookId);
            if (result is null) {
                return BadRequest("Value is not exist!");
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Post([FromBody] BookAuthor value) {
            _logger.LogInformation("/api/Authorship : post request");
            var IsExistNewValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == value.AuthorId && x.BookId == value.BookId) is not null;
            if (!IsExistNewValue) {
                _bookAuthorRepository.Insert(value);
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Put([FromBody] BookAuthor value) {
            _logger.LogInformation("/api/Authorship : put request");
            var oldValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == value.AuthorId && x.BookId == value.BookId);
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                _bookAuthorRepository.Update(value);
                _unitOfWork.SaveChanges();
                return Ok("This value is update!");
            }
            return Ok("This value is actually");
        }

        [HttpDelete("{authorId:int}/{bookId:int}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Delete(int authorId, int bookId) {
            _logger.LogInformation("/api/Authorship : delete request");
            var removedValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == authorId && x.BookId == bookId);
            if (removedValue is null) {
                return Ok("This value was deleted!");
            }
            _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _bookAuthorRepository.Delete(removedValue!);
            _unitOfWork.SaveChanges();
                return Ok("This value is deleted!");
        }
    }
}
