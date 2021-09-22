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
    public class ReviewController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IReviewerRepository _reviewerRepository;
        private IBookRepository _bookRepository;
        public ReviewController(IReviewRepository reviewRepository,IReviewerRepository reviewerRepository, IBookRepository bookRepository)
        {
            this._reviewRepository = reviewRepository;
            this._reviewerRepository = reviewerRepository;
            this._bookRepository = bookRepository;

            Console.WriteLine("Entre al constructor");
        }
        /*
        public ReviewController (IReviewRepository reviewRepository)
        {
            this._reviewRepository = reviewRepository;
        }
        */
        //api/reviews
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ICollection<ReviewDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetReviews()
        {
            var listReviewsBd = _reviewRepository.getReviews().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<ReviewDto> listReviewDto = new List<ReviewDto>();

            foreach(var review in listReviewsBd)
            {
                listReviewDto.Add(new ReviewDto()
                {
                    reviewId = review.Id,
                    reviwerText = review.ReviewText,
                    headline = review.Headline,
                    rating = review.Rating
                });
            }

            return Ok(listReviewDto);
        }

        //api/reviews/reviewId
        [HttpGet("{reviewId}",Name = "GetReview")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.reviewExists(reviewId))
                return NotFound();

            var reviewBd = _reviewRepository.getReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDto = new ReviewDto()
            {
                reviewId = reviewBd.Id,
                reviwerText = reviewBd.ReviewText,
                headline = reviewBd.Headline,
                rating = reviewBd.Rating
            };

            return Ok(reviewDto);
        }

        //api/reviews/reviewId/books
        [HttpGet("{reviewId}/books")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.reviewExists(reviewId))
                return NotFound();
            var bookDb = _reviewRepository.getBookOfAReview(reviewId);

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

        //api/reviews/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<ReviewDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsForABook(int bookId)
        {
            //FALTA IMPLEMENTAR
            //If books exists
            var reviewsBd = _reviewRepository.getReviewsOfABook(bookId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<ReviewDto> listReviewsDto = new List<ReviewDto>();

            foreach(var review in reviewsBd)
            {
                listReviewsDto.Add(new ReviewDto()
                {
                    reviewId = review.Id,
                    headline = review.Headline,
                    reviwerText = review.ReviewText,
                    rating = review.Rating
                });
            }

            return Ok(listReviewsDto);
        }

        //Faltan funciones por implementar y probar las de Reviewer

        //api/reviews
        [HttpPost]
        [ProducesResponseType(201,Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody]Review reviewToCreate)
        {
            //FALTA IMPLEMENTAR
            if (reviewToCreate == null)
                return BadRequest();

            if(!_reviewerRepository.reviewerExists(reviewToCreate.Reviewer.Id))
            {
                ModelState.AddModelError("", $"The reviewer does not exist.");
                return StatusCode(404, ModelState);
            }
            if(!_bookRepository.BookExists(reviewToCreate.Book.Id))
            {
                ModelState.AddModelError("", $"The book does not exists.");
                return StatusCode(404, ModelState);
            }

            var reviewerDb = _reviewerRepository.getReviewer(reviewToCreate.Reviewer.Id);
            var bookDb = _bookRepository.getBook(reviewToCreate.Book.Id);

            reviewToCreate.Book = bookDb;
            reviewToCreate.Reviewer = reviewerDb;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", $"There was an errror trying to create {reviewToCreate.Headline}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReview", new { reviewId = reviewToCreate.Id }, reviewToCreate);
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview([FromBody]Review reviewToUpdate)
        {
            if (reviewToUpdate == null)
                return BadRequest();

            if(!_reviewRepository.reviewExists(reviewToUpdate.Id))
            {
                ModelState.AddModelError("", $"The review {reviewToUpdate.Headline}, does not exists.");
            }

            if(!_bookRepository.BookExists(reviewToUpdate.Book.Id))
            {
                ModelState.AddModelError("", $"The book does not exist!");
            }

            if(!_reviewerRepository.reviewerExists(reviewToUpdate.Reviewer.Id))
            {
                ModelState.AddModelError("", $"The reviewer does not exist!");
            }

            if (!ModelState.IsValid)
                return StatusCode(404,ModelState);

            reviewToUpdate.Book = _bookRepository.getBook(reviewToUpdate.Book.Id);
            reviewToUpdate.Reviewer = _reviewerRepository.getReviewer(reviewToUpdate.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.UpdateReview(reviewToUpdate))
            {
                ModelState.AddModelError("", $"Something went wrong trying to update {reviewToUpdate.Headline} review.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.reviewExists(reviewId))
                return NotFound();

            var reviewBd = _reviewRepository.getReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_reviewRepository.DeleteReview(reviewBd))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewBd.Headline}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
