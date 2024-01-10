using Microsoft.AspNetCore.Mvc;
using uyg1.Models;
using uyg1.ViewModels;

namespace uyg1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CategoryListAjax()
        {
            var categoryModels = _context.Categories.Select(x => new CategoryViewModel()
            {
                CategoryID = x.CategoryID,
                Name = x.Name,
            }).ToList();

            return Json(categoryModels);
        }

        public IActionResult CategoryByIdAjax(int id)
        {
            var categoryModel = _context.Categories
                .Where(s => s.CategoryID == id)
                .Select(x => new CategoryViewModel()
                {
                    CategoryID = x.CategoryID,
                    Name = x.Name,
                })
                .SingleOrDefault();

            return Json(categoryModel);
        }

        [HttpPost]
        public IActionResult CategoryAddEditAjax(CategoryViewModel model)
        {
            var result = new SonucModel();
            if (model.CategoryID == 0)
            {
                if (_context.Categories.Any(c => c.Name == model.Name))
                {
                    result.Status = false;
                    result.Message = "Girilen Başlık Kayıtlıdır!";
                    return Json(result);
                }

                var category = new Category();
                category.Name = model.Name;
                _context.Categories.Add(category);
                _context.SaveChanges();
                result.Status = true;
                result.Message = "Kategori Eklendi";
            }
            else
            {
                var category = _context.Categories.FirstOrDefault(x => x.CategoryID == model.CategoryID);
                category.Name = model.Name;
                _context.SaveChanges();
                result.Status = true;
                result.Message = "Kategori Güncellendi";
            }

            return Json(result);
        }

        public IActionResult CategoryRemoveAjax(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.CategoryID == id);
            _context.Categories.Remove(category);
            _context.SaveChanges();

            var result = new SonucModel();
            result.Status = true;
            result.Message = "Kategori Silindi";
            return Json(result);
        }
    }
}
