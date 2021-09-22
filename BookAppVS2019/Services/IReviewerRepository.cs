using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> getReviewers();
        Reviewer getReviewer(int reviewerId);
        ICollection<Review> getReviewsByReviewer(int reviewerId);
        Reviewer getReviewerOfAReview(int reviewId);
        bool reviewerExists(int reviewerId);

        bool CreateReviewer(Reviewer reviewer);
        bool UpdateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);
        bool Save();
    }
}
