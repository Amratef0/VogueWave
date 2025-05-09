using Online_Store.Models;
using System.Collections.Generic;
namespace Online_Store.Interface
{
    public interface ICategoriesRepository
    {
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        void DeleteCategoryById(int id);
        List<Category> GetAllCategories();
        Category GetCategoryById(int id);
        void SaveCategoryChanges();
    }
}
