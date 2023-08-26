using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace WebAPI.Controllers
{
    [Route("api/Authorship")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private readonly ILogger<BookAuthorsController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<BookAuthor> _bookAuthorRepository;
        public BookAuthorsController(ILogger<BookAuthorsController> logger,
                                     IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookAuthorRepository = _unitOfWork.GetRepository<BookAuthor>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("BookAuthorsController", "NLog injected into BookAuthorsController");
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_bookAuthorRepository.GetPagedList(pageIndex: PageIndex,
                                                          include: i => i.Include(x => x.Book)
                                                                         .Include(x => x.Author),
                                                          pageSize: 1000).Items);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{authorId:int}/{bookId:int}")]
        public async Task<IActionResult> GetItem(Guid authorId, Guid bookId)
        {
            try
            {
                _logger.LogInformation("GET REQUEST IDs");
                var result = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == authorId && x.BookId == bookId);
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
        public async Task<IActionResult> InsertElement([FromBody] BookAuthor value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var IsExistNewValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == value.AuthorId && x.BookId == value.BookId) is not null;
                if(!IsExistNewValue)
                {
                    _bookAuthorRepository.Insert(value);
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
        public async Task<IActionResult> UpdateElement([FromBody] BookAuthor value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");
                var oldValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == value.AuthorId && x.BookId == value.BookId);
                if(oldValue is null)
                {
                    BadRequest("Values is not exist!");
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _bookAuthorRepository.Update(value);
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

        [HttpDelete("{authorId:int}/{bookId:int}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteElement(Guid authorId, Guid bookId)
        {
            try
            {
                _logger.LogInformation("DELETE REQUEST");
                var removedValue = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.AuthorId == authorId && x.BookId == bookId);
                if(removedValue is null)
                {
                    return Ok("This value was deleted!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _bookAuthorRepository.Delete(removedValue!);
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
