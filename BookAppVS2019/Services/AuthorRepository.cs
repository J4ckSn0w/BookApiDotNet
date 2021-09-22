using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private BookDbContext _authorContext;

        public AuthorRepository(BookDbContext authorContext)
        {
            this._authorContext = authorContext;
        }

        public bool authorExists(int authorId)
        {
            return this._authorContext.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Author author)
        {
            _authorContext.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _authorContext.Remove(author);
            return Save();
        }

        public Author getAuthor(int authorId)
        {
            return this._authorContext.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> getAuthors()
        {
            return this._authorContext.Authors.ToList();
        }

        public ICollection<Author> getAuthorsOfABook(int bookId)
        {
            return this._authorContext.BookAuthors.Where(ba => ba.BookId == bookId).Select(a => a.Author).ToList();
        }

        public ICollection<Book> getBooksByAuthor(int authorId)
        {
            //REVISAR
            return this._authorContext.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(b => b.Book).ToList();
        }

        public bool Save()
        {
            var saved = _authorContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            _authorContext.Update(author);
            return Save();
        }
    }
}
