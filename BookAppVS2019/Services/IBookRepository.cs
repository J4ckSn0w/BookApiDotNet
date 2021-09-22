using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public interface IBookRepository
    {
        ICollection<Book> getBooks();
        Book getBook(int bookId);
        Book getBook(string bookIsbn);
        decimal getBookRating(int bookId);
        bool BookExists(int bookId);
        bool BookExists(string bookIsbn);
        bool IsDuplicateIsbn(int bookId, string bookIsbn);

        bool CreateBook(Book bookToCreate, List<int> authors, List<int> categories);
        bool UpdateBook(Book bookToUpdate, List<int> authors, List<int> categories);
        bool DeleteBook(Book bookToDelete);
        bool Save();
        
    }
}
