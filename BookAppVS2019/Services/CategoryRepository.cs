using BookAppVS2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private BookDbContext _categoryContext;

        public CategoryRepository(BookDbContext categoryContext)
        {
            this._categoryContext = categoryContext;
        }

        public bool categoryExist(int idCategory)
        {
            return this._categoryContext.Categories.Any(c => c.Id == idCategory);
        }

        public bool CreateCategory(Category category)
        {
            _categoryContext.Categories.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _categoryContext.Categories.Remove(category);
            return Save();
        }

        public ICollection<Book> getBooksForCategory(int idCategory)
        {
            return this._categoryContext.BooksCategories.Where(bc => bc.CategoryId == idCategory).Select(b => b.Book).ToList();
        }

        public ICollection<Category> getCategories()
        {
            return this._categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> getCategoriesOfABook(int idBook)
        {
            //return _categoryContext.Categories.Where(c => c.BookCategories.Where(bc => bc.BookId == idBook)).ToList();
            return _categoryContext.BooksCategories.Where(bc => bc.BookId == idBook).Select(C => C.Category).ToList();
            //return null;
        }

        public Category getCategory(int idCategory)
        {
            return this._categoryContext.Categories.Where<Category>(c => c.Id == idCategory).FirstOrDefault();
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            var Category = _categoryContext.Categories
                .Where(c => c.Name.Trim().ToUpper() == categoryName.Trim().ToUpper() && c.Id != categoryId).FirstOrDefault();

            return Category == null ? false : true;
        }

        public bool Save()
        {
            var saved =_categoryContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _categoryContext.Categories.Update(category);
            return Save();
        }
    }
}
