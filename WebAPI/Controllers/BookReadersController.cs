using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using WebAPI.Filter;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/History")]
    [LocalAuthorization]
    public class BookReadersController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<BookReader> _bookReadersRepository;
        private IRepository<User> _userRepository;
        public BookReadersController(ILogger<AuthorsController> logger,
                                     IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookReadersRepository = _unitOfWork.GetRepository<BookReader>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _logger.LogDebug("BookReadersController", "NLog injected into BookReadersController");
        }
        [HttpGet]
        public async Task<IActionResult> GetPage(int PageIndex = 0)
        {
            try
            {
                _logger.LogInformation("GET REQUEST");
                return Ok(_bookReadersRepository.GetPagedList(pageIndex: PageIndex,
                                                           include: i => i.Include(x => x.Book)
                                                                          .Include(x => x.Reader),
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
                int count = _bookReadersRepository.Count();
                return Ok(count % 20 == 0 ? (count / 20) : ((count / 20) + 1));
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
                var result = _bookReadersRepository.Find(id);
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
        public async Task<IActionResult> InsertElement([FromBody] BookReader value)
        {
            try
            {
                _logger.LogInformation("POST REQUEST");
                var newValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x.Equals(value) && x.DateTimeEnd == value.DateTimeEnd && x.DateTimeStart == value.DateTimeStart);
                var IsExistNewValue = newValue is not null;
                if(!IsExistNewValue)
                {
                    _bookReadersRepository.Insert(value);
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
        public async Task<IActionResult> UpdateElement([FromBody] BookReader value)
        {
            try
            {
                _logger.LogInformation("PUT REQUEST");

                var identity = (ClaimsIdentity)User.Identity!;
                IEnumerable<Claim> claims = identity!.Claims;
                User? user = _userRepository.Find(int.Parse(claims.First().Value));

                var oldValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x == value);
                if(oldValue is null)
                {
                    BadRequest("Values is not exist!");
                }
                _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                bool IsEqualOldValue = oldValue!.Equals(value);
                if(!IsEqualOldValue)
                {
                    _bookReadersRepository.Update(value);
                    await _unitOfWork.SaveChangesAsync();
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
        [HttpDelete("{readerId:int}/{bookId:int}/{start:datetime}/{end:datetime}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteElement(int readerId, int bookId, DateTime start, DateTime end)
        {
            try
            {
                _logger.LogInformation("DELETE REQUEST");
                var removedValue = _bookReadersRepository.GetFirstOrDefault(predicate: x => x.BookId == bookId
                                                                                    && x.ReaderId == readerId
                                                                                    && x.DateTimeStart == start
                                                                                    && x.DateTimeEnd == end);
                if(removedValue is null)
                {
                    return Ok("This value was deleted!");
                }
                _unitOfWork.DbContext.Entry(removedValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
                _bookReadersRepository.Delete(removedValue!);
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
