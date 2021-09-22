using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public interface IAuthorRepository
    {
        ICollection<Author> getAuthors();
        Author getAuthor(int authorId);
        ICollection<Author> getAuthorsOfABook(int bookId);
        ICollection<Book> getBooksByAuthor(int authorId);
        bool authorExists(int authorId);

        bool CreateAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();
    }
}
