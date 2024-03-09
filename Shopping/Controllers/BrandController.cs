using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _context;
        public BrandController(DataContext context)
        {
            _context = context;

        }
        public IActionResult Index(string Alias="")
        {
            BrandModel brand = _context.Brands.Where(c => c.Alias == Alias).FirstOrDefault();
            if( brand == null )
            {
                return RedirectToAction("Index");

            }
            var productByBrand = _context.Products.Where(c => c.BrandId == brand.Id);
            

            return View(productByBrand.OrderBy(c=>c.BrandId).ToList());
        }
    }
}
