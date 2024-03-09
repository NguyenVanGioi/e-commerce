using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;

        }
        public IActionResult Index(string Alias="")
        {
            CategoryModel category = _context.Categories.Where(c => c.Alias == Alias).FirstOrDefault();
            if( category == null )
            {
                return RedirectToAction("Index");

            }
            var productByCategory = _context.Products.Where(c => c.CategoryId == category.Id);
            

            return View(productByCategory.OrderBy(c=>c.CategoryId).ToList());
        }
    }
}
