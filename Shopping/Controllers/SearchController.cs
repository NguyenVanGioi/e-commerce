using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class SearchController : Controller
    {
        private readonly DataContext _context;

        public SearchController(DataContext context)
        {
            _context = context;
        }

        // GET: /Search
        public async Task<IActionResult> Index(string searchString)
        {
            var products = await _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Where(x => x.Name.Contains(searchString))
                .ToListAsync();

            return View(products);
        }
    }

}
