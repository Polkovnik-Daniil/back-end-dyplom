﻿using DBManager.Pattern.Interface;
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
        private IRepository<BookGenre> _bookGenreRepository;


        public BooksController(ILogger<BooksController> logger,
                              IServiceProvider serviceProvider,
                              ITokenService tokenService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bookRepository = _unitOfWork.GetRepository<Book>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _genreRepository = _unitOfWork.GetRepository<Genre>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            _bookGenreRepository = _unitOfWork.GetRepository<BookGenre>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
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
            _unitOfWork.DbContext.Entry(value!).State = EntityState.Detached;
            var IsExistNewValue = _bookRepository.GetFirstOrDefault(predicate: x=>x.Realise == value.Realise && x.Title == value.Title && x.Quantity == value.Quantity) is not null;
            if (!IsExistNewValue) {
                var temp = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
                value.Genres = null;
                _bookRepository.Insert(value);
                _unitOfWork.SaveChanges();
                if (temp is not null) {
                    var bookId = _bookRepository.GetFirstOrDefault(predicate: x => x.Realise == value.Realise && x.Title == value.Title && x.Quantity == value.Quantity).Id;
                    for (int i = 0; i < temp.Count; i++) {
                        var isExist = _bookGenreRepository.GetFirstOrDefault(predicate: x => x.BookId == temp[i].Id && x.GenreId == temp[i].Id) is not null;
                        if (!isExist) {
                            _bookGenreRepository.Insert(new BookGenre() { BookId = bookId, GenreId = temp[i].Id });
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                return Ok("This value was added!");
            }
            return Ok("This value is exist!");
        }

        [HttpPut, Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Put([FromBody] Book value) {
            _logger.LogInformation("/api/Book : put request");
            var oldValue = _bookRepository.Find(value.Id);
            _unitOfWork.DbContext.Entry(oldValue!).State = EntityState.Detached; //убираю отслеживание, для того, чтобы можно было обновить значение
            if (oldValue is null) {
                BadRequest("Values is not exist!");
            }
            value.Genres = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
            bool IsEqualOldValue = oldValue!.Equals(value);
            if (!IsEqualOldValue) {
                value.Genres = value.Genres != null && value.Genres.Count != 0 ? value.Genres.Select(g => _genreRepository.GetFirstOrDefault(predicate: x => x.Name == g.Name)).ToList() : null;
                IList<BookGenre> deletedList = _bookGenreRepository.GetPagedList(predicate: x => x.BookId == value.Id, pageSize: _bookGenreRepository.Count()).Items;
                for (int i = 0; i < value.Genres.Count; i++) {
                    var isExist = _bookGenreRepository.GetFirstOrDefault(predicate: x => x.BookId == value.Id && x.GenreId == value.Genres[i].Id) is not null;
                    if (!isExist) {
                        _bookGenreRepository.Insert(new BookGenre() { BookId = value.Id, GenreId = value.Genres[i].Id });
                    } else {
                        var deletedValue = deletedList.Where(x => x.GenreId == value.Genres[i].Id && x.BookId == value.Id).First();
                        value.Genres.Remove(value.Genres[i]);
                        deletedList.Remove(deletedValue);
                        i--;
                    }
                }
                _bookGenreRepository.Delete(deletedList);
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
