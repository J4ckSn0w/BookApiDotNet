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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private IAuthorRepository _authorRepository;
        private ICountryRepository _countryRepository;
        private IBookRepository _bookRepository;

        public AuthorController(IAuthorRepository authorRepository, ICountryRepository countryRepository, IBookRepository bookRepository)
        {
            this._authorRepository = authorRepository;
            this._countryRepository = countryRepository;
            this._bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<AuthorDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetAuthors()
        {
            var authorsDB = _authorRepository.getAuthors().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<AuthorDto> listAuthorDto = new List<AuthorDto>();

            foreach (var author in authorsDB)
            {
                listAuthorDto.Add(new AuthorDto()
                {
                    authorId = author.Id,
                    firstName = author.FirstName,
                    lastName = author.LastName
                });
            }

            return Ok(listAuthorDto);
        }

        //api/author/authorId
        [HttpGet("{authorId}", Name = "getAuthor")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.authorExists(authorId))
                return NotFound();
            var authorBd = _authorRepository.getAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthorDto authorDto = new AuthorDto()
            {
                authorId = authorBd.Id,
                firstName = authorBd.FirstName,
                lastName = authorBd.LastName
            };

            return Ok(authorDto);
        }

        //api/author/authorId/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(200, Type = typeof(ICollection<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.authorExists(authorId))
                return NotFound();

            var booksBd = _authorRepository.getBooksByAuthor(authorId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<BookDto> listBooksDto = new List<BookDto>();

            foreach (var book in booksBd)
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

        //api/author/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthorOfABook(int bookId)
        {
            //VALIDAR
            //if bookexists

            var authorsBd = _authorRepository.getAuthorsOfABook(bookId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<AuthorDto> listAuthorsDto = new List<AuthorDto>();

            foreach (var author in authorsBd)
            {
                listAuthorsDto.Add(new AuthorDto()
                {
                    authorId = author.Id,
                    firstName = author.FirstName,
                    lastName = author.LastName
                });
            }

            return Ok(listAuthorsDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate)
        {
            if (authorToCreate == null)
                return BadRequest();
            if (!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", $"The country does not exist.");
                return StatusCode(400, ModelState);
            }

            var authorsCountry = _countryRepository.GetCountry(authorToCreate.Country.Id);

            authorToCreate.Country = authorsCountry;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"There was an error trying to create {authorToCreate.FirstName} {authorToCreate.LastName}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("getAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId,[FromBody]Author authorToUpdate)
        {
            if (authorToUpdate == null)
                return BadRequest();
            if(authorToUpdate.Id != authorId)
            {
                ModelState.AddModelError("", $"The ids dont match.");
                return StatusCode(400, ModelState);
            }
            if (!_authorRepository.authorExists(authorId))
            {
                ModelState.AddModelError("", $"Author does not exist.");
                return StatusCode(404, ModelState);
            }
            if(!_countryRepository.CountryExists(authorToUpdate.Country.Id))
            {
                ModelState.AddModelError("", $"Country does not exist.");
                return StatusCode(404, ModelState);
            }

            authorToUpdate.Country = _countryRepository.GetCountry(authorToUpdate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_authorRepository.UpdateAuthor(authorToUpdate))
            {
                ModelState.AddModelError("", $"There was a problem trying to update {authorToUpdate.FirstName} {authorToUpdate.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{authorId}")]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if(!_authorRepository.authorExists(authorId))
            {
                ModelState.AddModelError("", $"The author does not exist.");
                return StatusCode(404, ModelState);
            }

            var authorToDelete = _authorRepository.getAuthor(authorId);

            if(_authorRepository.getBooksByAuthor(authorToDelete.Id).Count > 0)
            {
                ModelState.AddModelError("", $"We can't delete the author {authorToDelete.FirstName} {authorToDelete.LastName}, because he is asssociated wwith at least one book.");
                return StatusCode(209, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"There was an error trying to delete {authorToDelete.FirstName} {authorToDelete.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
