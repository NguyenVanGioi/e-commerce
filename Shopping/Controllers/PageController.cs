using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Repository;
using X.PagedList;

namespace Shopping.Controllers
{
    public class PageController : Controller
    {
        private readonly DataContext _context;
        public PageController(DataContext context)
        {
            _context = context;

        }
        public IActionResult Index(int page = 1)
        {
            int pageSize = 8;
            var products = _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Brand).OrderByDescending(x => x.Name).ToPagedList(page, pageSize);
            return View(products);
        }
    }
}
