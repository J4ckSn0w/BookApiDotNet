using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> getCategories();
        Category getCategory(int idCategory);
        ICollection<Category> getCategoriesOfABook(int idBook);
        ICollection<Book> getBooksForCategory(int idCategory);
        bool categoryExist(int idCategory);
        bool IsDuplicateCategoryName(int categoryId, string categoryName);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
