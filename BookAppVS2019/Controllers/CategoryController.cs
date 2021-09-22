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
    [Route("/api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        //api/category
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public IActionResult getCategories()
        {
            List<CategoryDto> categoriesDtos = new List<CategoryDto>();

            var categories = _categoryRepository.getCategories().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var category in categories)
            {
                categoriesDtos.Add(new CategoryDto()
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }
            return Ok(categoriesDtos);
        }

        //api/category/{idCategory}
        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public IActionResult getCategory(int categoryId)
        {
            if (!_categoryRepository.categoryExist(categoryId))
                return NotFound();

            var category = _categoryRepository.getCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        //api/category/bookId/{bookId}
        [HttpGet("bookId/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public IActionResult getCategoriesOfABook(int bookId)
        {
            List<CategoryDto> categoriesDto = new List<CategoryDto>();

            var categories = _categoryRepository.getCategoriesOfABook(bookId).ToList();

            if (categories == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var category in categories)
            {
                categoriesDto.Add(new CategoryDto()
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoriesDto);
        }


        [HttpGet("book/categoryId/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult getBooksForCategory(int categoryId)
        {
            if (!_categoryRepository.categoryExist(categoryId))
                return NotFound();
            var books = _categoryRepository.getBooksForCategory(categoryId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach (var book in books)
            {
                booksDto.Add(new BookDto
                {
                    bookId = book.Id,
                    Isbn = book.Isbn,
                    title = book.Title,
                    datePublished = book.DatePublished
                });
            }

            return Ok(booksDto);
        }

        //api/category
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            if (category == null)
                return BadRequest();

            if (_categoryRepository.IsDuplicateCategoryName(category.Id, category.Name))
            {
                ModelState.AddModelError("", $"The Category {category.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wront trying to add {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);
        }

        //api/category/categoryId
        [HttpPut("{categoryId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory([FromBody] Category category, int categoryId)
        {
            if (category == null)
                return BadRequest();
            if (!_categoryRepository.categoryExist(categoryId))
                return NotFound();
            if (category.Id != categoryId)
                return BadRequest();
            if (_categoryRepository.IsDuplicateCategoryName(categoryId, category.Name))
            {
                ModelState.AddModelError("", $"The Category { category.Name } already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong trying to update {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/category/categoryId
        [HttpDelete("{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.categoryExist(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.getCategory(categoryId);

            if(_categoryRepository.getBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"The category {categoryToDelete.Name} can't be delete, cause at least one book has.");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", $"There was an error trying to delete {categoryToDelete}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
