using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public interface IReviewRepository
    {
        ICollection<Review> getReviews();
        Review getReview(int reviewId);
        ICollection<Review> getReviewsOfABook(int bookId);
        Book getBookOfAReview(int reviewId);
        bool reviewExists(int reviewId);
        bool CreateReview(Review review);
        bool UpdateReview(Review review);
        bool DeleteReview(Review review);
        bool DeleteReviews(List<Review> reviews);
        bool Save();
    }
}
