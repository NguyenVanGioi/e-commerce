using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class PostController : Controller
    {
        private readonly DataContext _context;
        public PostController(DataContext context)
        {
            _context = context;

        }


        public IActionResult Index()
        {



            var tintuc = _context.Posts.AsNoTracking().OrderBy(x => x.Id).ToList();

            return View(tintuc);
        }

		
		public async Task<IActionResult> Details(int id)
		{
			var tintuc = await _context.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

			if (tintuc == null)
			{
				return RedirectToAction("Index");
			}

			var lsBaivietlienquan = await _context.Posts
				.AsNoTracking()
				.Where(x => x.Published && x.Id != id)
				.Take(10)
				.OrderBy(x => x.DateCreated)
				.ToListAsync();

			ViewBag.Baivietlienquan = lsBaivietlienquan;

			return View(tintuc);
		}

	}
}
