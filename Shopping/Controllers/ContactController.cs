using Microsoft.AspNetCore.Mvc;
using Shopping.Repository;

namespace Shopping.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _context;
        public ContactController(DataContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
