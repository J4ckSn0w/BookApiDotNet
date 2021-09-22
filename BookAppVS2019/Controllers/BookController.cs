using BookAppVS2019.Dtos;
using BookAppVS2019.Models;
using BookAppVS2019.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private IBookRepository _bookRepository;

        private IAuthorRepository _authorRepository;
        private ICategoryRepository _categoryRepository;
        private IReviewRepository _reviewRepository;

        public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            this._bookRepository = bookRepository;
            this._authorRepository = authorRepository;
            this._categoryRepository = categoryRepository;
            this._reviewRepository = reviewRepository;
        }


        //api/books
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ICollection<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooks()
        {
            var booksDb = _bookRepository.getBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<BookDto> listBooksDto = new List<BookDto>();

            foreach (var book in booksDb)
            {
                listBooksDto.Add(new BookDto()
                {
                    bookId = book.Id,
                    Isbn = book.Isbn,
                    title = book.Title,
                    datePublished = book.DatePublished
                });
            }

            return Ok(listBooksDto);
        }

        //api/books/bookId
        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var bookDb = _bookRepository.getBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                bookId = bookDb.Id,
                Isbn = bookDb.Isbn,
                title = bookDb.Title,
                datePublished = bookDb.DatePublished
            };

            return Ok(bookDto);
        }

        //api/book/Isbn/bookIsbn
        [HttpGet("Isbn/{bookIsbn}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(string bookIsbn)
        {
            if (!_bookRepository.BookExists(bookIsbn))
                return NotFound();

            var bookDb = _bookRepository.getBook(bookIsbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                bookId = bookDb.Id,
                Isbn = bookDb.Isbn,
                title = bookDb.Title,
                datePublished = bookDb.DatePublished
            };

            return Ok(bookDto);
        }

        //api/books/bookId/rating
        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var rating = _bookRepository.getBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }

        private StatusCodeResult ValidateBook(List<int> authorId, List<int> categoriesId, Book book)
        {
            if (book == null || authorId.Count == 0 || categoriesId.Count == 0)
            {
                ModelState.AddModelError("", $"Missing book, author or category");
                return BadRequest();
            }

            if (_bookRepository.IsDuplicateIsbn(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", $"Duplicate ISBN");
                return StatusCode(422);
            }

            foreach (var id in authorId)
            {
                if (!_authorRepository.authorExists(id))
                {
                    ModelState.AddModelError("", $"Author Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in categoriesId)
            {
                if (!_categoryRepository.categoryExist(id))
                {
                    ModelState.AddModelError("", $"Category Not Found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", $"Critical Error.");
                return BadRequest();
            }

            return NoContent();
        }

        //api/books?authId=1&authId=2&catId=1&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody] Book bookToCreate)
        {
            var statusCode = ValidateBook(authId, catId, bookToCreate);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode, ModelState);

            if (!_bookRepository.CreateBook(bookToCreate, authId, catId))
            {
                ModelState.AddModelError("", $"Something went wrong saving the book {bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);
        }

        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int bookId, [FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody] Book bookToUpdate)
        {
            var statusCode = ValidateBook(authId, catId, bookToUpdate);

            if (bookId != bookToUpdate.Id)
                return BadRequest();

            if (!_bookRepository.BookExists(bookToUpdate.Id))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode, ModelState);

            if (!_bookRepository.UpdateBook(bookToUpdate, authId, catId))
            {
                ModelState.AddModelError("", $"Something went wrong saving the book {bookToUpdate.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var reviewsToDelete = _reviewRepository.getReviewsOfABook(bookId);
            var bookToDelete = _bookRepository.getBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting reviews");
                return StatusCode(500, ModelState);
            }

            if(!_bookRepository.DeleteBook(bookToDelete))
            {
                ModelState.AddModelError("", $"There was a problem trying to delete book {bookToDelete.Title}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
