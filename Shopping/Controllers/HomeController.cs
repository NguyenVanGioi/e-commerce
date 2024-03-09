using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace Shopping.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private UserManager<AppUserModel> _userManager;

        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /*  public IActionResult Index()
          {
              var product = _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Brand).OrderBy(x => x.Id).ToList();
              return View(product);
          }*/
        public IActionResult Index(int page = 1)
        {
            int pageSize = 8;
            var products = _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Brand).OrderByDescending(x => x.Name).ToPagedList(page, pageSize);
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.AsNoTracking().Include(x => x.Comments).Include(x => x.Category).Include(x => x.Brand).SingleOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var lsSanphamlienquan = await _context.Products
                .AsNoTracking()
                .Where(x => x.CategoryId == product.CategoryId & x.Id != id && x.Active == true)
                .Take(4)
                .OrderBy(x => x.DateCreated)
                .ToListAsync();

            ViewBag.Sanphamlienquan = lsSanphamlienquan;



            return View(product);
        }

        public async Task<IActionResult> AddComment(int productId, string message, int rating)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, xử lý tùy ý (ví dụ: chuyển hướng đến trang đăng nhập)
                return RedirectToAction("Login", "Account");
            }

            string userId = _userManager.GetUserId(User);

            // Lấy thông tin người dùng
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Người dùng không tồn tại, xử lý tùy ý
                return RedirectToAction("Index");
            }

            // Lấy tên và email của người dùng
            var userName = user.UserName;
            var userEmail = user.Email;

            // Tiếp tục thêm bình luận với thông tin người dùng
            var comment = new CommentModel
            {
                ProductId = productId,
                UserId = userId,
                Name = userName, 
                Email = userEmail,
                Message = message,
                rate = rating,
                DateCreated = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Redirect(Request.Headers["Referer"].ToString());
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
