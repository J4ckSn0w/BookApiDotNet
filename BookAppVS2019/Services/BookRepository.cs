using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookContext;

        public BookRepository(BookDbContext bookContext)
        {
            this._bookContext = bookContext;
        }

        public bool BookExists(int bookId)
        {
            return _bookContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExists(string bookIsbn)
        {
            return _bookContext.Books.Any(b => b.Isbn == bookIsbn);
        }

        public bool CreateBook(Book bookToCreate, List<int> authors, List<int> categories)
        {

            var authorsIds = _bookContext.Authors.Where(a => authors.Contains(a.Id)).ToList();
            var categoriesIds = _bookContext.Categories.Where(c => categories.Contains(c.Id)).ToList();

            foreach(var author in authorsIds)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = bookToCreate
                };
                _bookContext.Add(bookAuthor);
            }

            foreach(var category in categoriesIds)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = bookToCreate
                };
                _bookContext.Add(bookCategory);
            }

            _bookContext.Add(bookToCreate);

            return Save();
        }

        public bool DeleteBook(Book bookToDelete)
        {
            _bookContext.Remove(bookToDelete);
            return Save();
        }

        public Book getBook(int bookId)
        {
            return _bookContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book getBook(string bookIsbn)
        {
            return _bookContext.Books.Where(b => b.Isbn == bookIsbn).FirstOrDefault();
        }

        public decimal getBookRating(int bookId)
        {
            var reviews = _bookContext.Reviews.Where(r => r.Book.Id == bookId);

            if (reviews.Count() <= 0)
                return 0;
            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public ICollection<Book> getBooks()
        {
            return _bookContext.Books.ToList();
        }

        public bool IsDuplicateIsbn(int bookId, string bookIsbn)
        {
            var book = _bookContext.Books.Where(b => b.Isbn.Trim().ToUpper() == bookIsbn.Trim().ToUpper() && b.Id != bookId).FirstOrDefault();

            return book == null ? false : true;
        }

        public bool Save()
        {
            var saved = _bookContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateBook(Book bookToUpdate, List<int> authors, List<int> categories)
        {
            var authorsIds = _bookContext.Authors.Where(a => authors.Contains(a.Id)).ToList();
            var categoriesIds = _bookContext.Categories.Where(c => categories.Contains(c.Id)).ToList();

            var bookAuthorsToDelete = _bookContext.BookAuthors.Where(ba => ba.BookId == bookToUpdate.Id).ToList();
            var bookCateogiresToDelete = _bookContext.BooksCategories.Where(bc => bc.BookId == bookToUpdate.Id).ToList();

            _bookContext.RemoveRange(bookAuthorsToDelete);
            _bookContext.RemoveRange(bookCateogiresToDelete);

            foreach (var author in authorsIds)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = bookToUpdate
                };
                _bookContext.Add(bookAuthor);
            }

            foreach (var category in categoriesIds)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = bookToUpdate
                };
                _bookContext.Add(bookCategory);
            }

            _bookContext.Update(bookToUpdate);

            return Save();
        }
    }
}
