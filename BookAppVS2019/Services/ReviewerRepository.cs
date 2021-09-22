using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public class ReviewerRepository : IReviewerRepository
    {

        private BookDbContext _reviewerContext;

        public ReviewerRepository(BookDbContext reviewerContext)
        {
            _reviewerContext = reviewerContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _reviewerContext.Reviewers.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _reviewerContext.Remove(reviewer);
            return Save();
        }

        public Reviewer getReviewer(int reviewerId)
        {
            return _reviewerContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public Reviewer getReviewerOfAReview(int reviewId)
        {
            //POSIBLEMENTE FALLA
            //return _reviewerContext.Reviewers.Where(r => r.Reviews.Select(rev => rev.Id == reviewId)).FirstOrDefault();
            var reviewerDb = _reviewerContext.Reviews.Where(r => r.Id == reviewId).Select(rev => rev.Reviewer).FirstOrDefault();
            return reviewerDb;
        }

        public ICollection<Reviewer> getReviewers()
        {
            return _reviewerContext.Reviewers.ToList();
        }

        public ICollection<Review> getReviewsByReviewer(int reviewerId)
        {
            return _reviewerContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool reviewerExists(int reviewerId)
        {
            return _reviewerContext.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool Save()
        {
            var saved =_reviewerContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _reviewerContext.Update(reviewer);
            return Save();
        }
    }
}
