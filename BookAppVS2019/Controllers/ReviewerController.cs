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
    public class ReviewerController : Controller
    {
        private IReviewerRepository _reviewerRespository;

        private IReviewRepository _reviewRepository;

        public ReviewerController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            this._reviewRepository = reviewRepository;
            this._reviewerRespository = reviewerRepository;
        }

        //api/reviewers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewers()
        {
            var reviewersBd = _reviewerRespository.getReviewers().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<ReviewerDto> reviewersDto = new List<ReviewerDto>();

            foreach (var reviewer in reviewersBd)
            {
                reviewersDto.Add(new ReviewerDto()
                {
                    reviewerId = reviewer.Id,
                    firstName = reviewer.FirstName,
                    lastName = reviewer.LastName
                });
            }

            return Ok(reviewersDto);
        }

        //api/reviewers/reviewerId
        [HttpGet("{reviewerId}",Name = "GetReviewer")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRespository.reviewerExists(reviewerId))
                return NotFound();
            var reviewerBd = _reviewerRespository.getReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                reviewerId = reviewerBd.Id,
                firstName = reviewerBd.FirstName,
                lastName = reviewerBd.LastName
            };
            return Ok(reviewerDto);
        }

        //api/reviewers/reviewId/review
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(ICollection<ReviewDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewByReviewer(int reviewerId)
        {
            if (!_reviewerRespository.reviewerExists(reviewerId))
                return NotFound();

            var listaReviewsBd = _reviewerRespository.getReviewsByReviewer(reviewerId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<ReviewDto> listaReviewsDto = new List<ReviewDto>();

            foreach (var review in listaReviewsBd)
            {
                listaReviewsDto.Add(new ReviewDto()
                {
                    reviewId = review.Id,
                    reviwerText = review.ReviewText,
                    headline = review.Headline,
                    rating = review.Rating
                });
            }

            return Ok(listaReviewsDto);
        }

        //api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            //Falta implementar _reviewRepository

            if (!_reviewRepository.reviewExists(reviewId))
                return NotFound();


            var reviewerBd = _reviewerRespository.getReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                reviewerId = reviewerBd.Id,
                firstName = reviewerBd.FirstName,
                lastName = reviewerBd.LastName
            };

            return Ok(reviewerDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody] Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRespository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong trying to create the reviewer " +
                    $"{reviewerToCreate.FirstName} {reviewerToCreate.LastName}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer reviewerToUpdate)
        {
            if (reviewerToUpdate == null)
                return BadRequest();
            if (reviewerToUpdate.Id != reviewerId)
                return BadRequest();

            if (!_reviewerRespository.reviewerExists(reviewerId))
            {
                //ModelState.AddModelError("", $"There is no Reviewer with that Id");
                //return StatusCode(404,ModelState);
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRespository.UpdateReviewer(reviewerToUpdate))
            {
                ModelState.AddModelError("", $"There was an error trying to update {reviewerToUpdate.FirstName} {reviewerToUpdate.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRespository.reviewerExists(reviewerId))
                return NotFound();
            var reviewerToDelete = _reviewerRespository.getReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest();

            if(!_reviewerRespository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"There was an error trying to Delete the reviewer {reviewerToDelete.FirstName} {reviewerToDelete.LastName}.");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
