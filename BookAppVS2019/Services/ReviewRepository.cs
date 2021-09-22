using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public class ReviewRepository : IReviewRepository
    {
        BookDbContext _reviewContext;

        public ReviewRepository(BookDbContext reviewContext)
        {
            this._reviewContext = reviewContext;
        }

        public bool CreateReview(Review review)
        {
            _reviewContext.Reviews.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _reviewContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _reviewContext.RemoveRange(reviews);
            return Save();
        }

        public Book getBookOfAReview(int reviewId)
        {
            return _reviewContext.Reviews.Where(r => r.Id == reviewId).Select(b => b.Book).FirstOrDefault();
        }

        public Review getReview(int reviewId)
        {
            return _reviewContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> getReviews()
        {
            return _reviewContext.Reviews.ToList();
        }

        public ICollection<Review> getReviewsOfABook(int bookId)
        {
            return _reviewContext.Reviews.Where(r => r.Book.Id == bookId).ToList();
        }

        public bool reviewExists(int reviewId)
        {
            return _reviewContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var saved = _reviewContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _reviewContext.Update(review);
            return Save();
        }
    }
}
