using Online_Store.Interface;
using Online_Store.Models;

namespace Online_Store.Repository
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly OnlineStoreContext _context;

        public CategoriesRepository(OnlineStoreContext context)
        {
            _context = context;
        }
        public void AddCategory(Category category)
        {
            _context.Add(category);
        }

        // تحديث تصنيف
        public void UpdateCategory(Category category)
        {
            _context.Update(category);
        }

        // حذف تصنيف
        public void DeleteCategory(Category category)
        {
            _context.Remove(category);
        }

        // حذف تصنيف بواسطة الـ ID
        public void DeleteCategoryById(int id)
        {
            Category category = GetCategoryById(id);
            _context.Remove(category);
        }

        // الحصول على جميع التصنيفات
        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        // الحصول على تصنيف بواسطة الـ ID
        public Category GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        // حفظ التغييرات في التصنيفات
        public void SaveCategoryChanges()
        {
            _context.SaveChanges();
        }
    }
}

