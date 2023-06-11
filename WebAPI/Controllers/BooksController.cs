using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebAPI.Filter;

namespace WebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [LocalAuthorization]
    public class BooksController : ControllerBase {
        private readonly ILogger<BooksController> _logger; //для логов
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<Book> _bookRepository;
        private IRepository<Genre> _genreRepository;
        private IRepository<Author> _authorRepository;
        private IRepository<BookGenre> _bookGenreRepository;
        private IRepository<BookAuthor> _bookAuthorRepository;


        public BooksController(ILogger<BooksController> logger,
                              IServiceProvider serviceProvider,
                              ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookRepository = _unitOfWork.GetRepository<Book>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _genreRepository = _unitOfWork.GetRepository<Genre>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _authorRepository = _unitOfWork.GetRepository<Author>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _bookGenreRepository = _unitOfWork.GetRepository<BookGenre>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _bookAuthorRepository = _unitOfWork.GetRepository<BookAuthor>() ?? throw new ArgumentNullException(nameof(_unitOfWork));

        }

        [HttpGet]
        public async Task<IList<Book>> GetPage(int PageIndex = 0) {
            _logger.LogInformation("/api/Book : get request");
            int count = _bookRepository.Count();
            return _bookRepository.GetPagedList(pageIndex: PageIndex,
                                                include: i => i.Include(x => x.Genres)
                                                               .Include(x => x.Authors),
                                                pageSize: 1000).Items;
        }
        [HttpGet, Route("CountPage")]
        public async Task<int> GetCountPage() {
            int count = await _bookRepository.CountAsync();
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
            var IsExistNewValue = _bookRepository.GetFirstOrDefault(predicate: x=>x.Realise == value.Realise && x.Title == value.Title && x.Quantity == value.Quantity) is not null;
            if (!IsExistNewValue) {
                var tempGenre = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
                var tempAuthor = value.Authors != null && value.Authors.Count != 0 ? value.Authors.Select(g => _authorRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
                tempGenre = tempGenre.Where(x => x != null).ToList();
                tempAuthor = tempAuthor.Where(x => x != null).ToList();
                value.Genres = null;
                value.Authors = null;
                _bookRepository.Insert(value);
                _unitOfWork.SaveChanges();
                var bookId = _bookRepository.GetFirstOrDefault(predicate: x => x.Realise == value.Realise && x.Title == value.Title && x.Quantity == value.Quantity).Id;
                if (tempGenre is not null) {
                    for (int i = 0; i < tempGenre.Count; i++) {
                        var isExist = _bookGenreRepository.GetFirstOrDefault(predicate: x => x.BookId == bookId && x.GenreId == tempGenre[i].Id) is not null;
                        if (!isExist) {
                            _bookGenreRepository.Insert(new BookGenre() { BookId = bookId, GenreId = tempGenre[i].Id });
                        }
                    }
                }
                _unitOfWork.SaveChanges();
                if (tempAuthor is not null) {
                    for (int i = 0; i < tempAuthor.Count; i++) {
                        var isExist = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.BookId == bookId && x.AuthorId == tempAuthor[i].Id) is not null;
                        if (!isExist) {
                            _bookAuthorRepository.Insert(new BookAuthor() { BookId = bookId, AuthorId = tempAuthor[i].Id });
                        }
                    }
                }
                _unitOfWork.SaveChanges();
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Put([FromBody] Book value) {
            _logger.LogInformation("/api/Book : put request");
            var oldValue = _bookRepository.Find(value.Id);
            _unitOfWork.DbContext.Entry(value!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            value.Genres = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
            var tempAuthor = value.Authors != null && value.Authors.Count != 0 ? value.Authors.Select(g => _authorRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
            value.Genres = value.Genres.Where(x => x != null).ToList();
            tempAuthor = tempAuthor.Where(x => x != null).ToList();
            value.Authors = null;
            bool IsEqualOldValue = oldValue!.Equals(value);
            if(!IsEqualOldValue) {
                value.Genres = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;

                IList<BookGenre> deletedList = _bookGenreRepository.GetPagedList(predicate: x => x.BookId == value.Id, pageSize: _bookGenreRepository.Count()).Items;
                if(value.Genres != null)
                {
                    for(int i = 0; i < value.Genres.Count; i++)
                    {
                        var isExist = _bookGenreRepository.GetFirstOrDefault(predicate: x => x.BookId == value.Id && x.GenreId == value.Genres[i].Id) is not null;
                        if(!isExist)
                        {
                            _bookGenreRepository.Insert(new BookGenre() { BookId = value.Id, GenreId = value.Genres[i].Id });
                            value.Genres.Remove(value.Genres[i]);
                            i--;
                        }
                        else
                        {
                            var deletedValue = deletedList.Where(x => x.GenreId == value.Genres[i].Id && x.BookId == value.Id).First();
                            value.Genres.Remove(value.Genres[i]);
                            deletedList.Remove(deletedValue);
                            i--;
                        }
                    }
                }
                
                _bookGenreRepository.Delete(deletedList);
                _bookRepository.Update(value);
                _unitOfWork.SaveChanges();
                value.Authors = tempAuthor;
                value.Authors = value.Authors != null && value.Authors.Count != 0 ? value.Authors.Select(g => _authorRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
                IList<BookAuthor> deletedListAuthors = _bookAuthorRepository.GetPagedList(predicate: x => x.BookId == value.Id, pageSize: _bookAuthorRepository.Count()).Items;
                if(value.Authors != null)
                {
                    for(int i = 0; i < value.Authors.Count; i++)
                    {
                        Console.WriteLine(i + "\n");
                        Console.WriteLine(value.Authors[i].Id + "\n");
                        _unitOfWork.DbContext.Entry(value!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение

                        var isExist = _bookAuthorRepository.GetFirstOrDefault(predicate: x => x.BookId == value.Id && x.AuthorId ==  value.Authors[i].Id) is not null;
                        if(!isExist)
                        {
                            _bookAuthorRepository.Insert(new BookAuthor() { BookId = value.Id, AuthorId = value.Authors[i].Id });
                            value.Authors.Remove(value.Authors[i]);
                            i--;
                        }
                        else
                        {

                            var deletedValue = deletedListAuthors.Where(x => x.AuthorId ==  value.Authors[i].Id && x.BookId == value.Id).First();
                            value.Authors.Remove(value.Authors[i]);
                            deletedListAuthors.Remove(deletedValue);
                            i--;
                        }
                    }
                    value.BookAuthors = null;
                }
                _bookAuthorRepository.Delete(deletedListAuthors);
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
