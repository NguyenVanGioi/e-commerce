using Microsoft.AspNetCore.Mvc;
using Shopping.Models.ViewModels;
using Shopping.Models;
using Shopping.Repository;

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Shopping.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager; 

        public CartController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            // Lấy UserId của người dùng hiện tại
            string userId = _userManager.GetUserId(User);

            
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
                                            ?.Where(c => c.UserId == userId)
                                            .ToList() ?? new List<CartItemModel>();
            // Lọc ra chỉ những sản phẩm thuộc về UserId của người dùng hiện tại
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems.Where(c => c.UserId == userId).ToList(),
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }



        public async Task<IActionResult> Add(int Id)
        {
            // Lấy UserId của người dùng hiện tại
            string userId = _userManager.GetUserId(User);

            ProductModel product = await _context.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            CartItemModel cartItem = cart.Where(c => c.Id == Id && c.UserId == userId).FirstOrDefault();

            if (cartItem == null)
            {
                // Nếu sản phẩm không tồn tại trong giỏ hàng, thêm mới với UserId của người dùng
                cart.Add(new CartItemModel(product) { UserId = userId });
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["message"] = "Thêm vào giỏ hàng thành công";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            string userId = _userManager.GetUserId(User);

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.Id == Id && c.UserId == userId).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;

            }
            else
            {
                cart.RemoveAll(p => p.Id == Id && p.UserId == userId);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            string userId = _userManager.GetUserId(User);

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.Id == Id && c.UserId == userId).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;

            }
            else
            {
                cart.RemoveAll(p => p.Id == Id && p.UserId == userId);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            string userId = _userManager.GetUserId(User);

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.Id == Id && p.UserId == userId);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }
            TempData["message"] = "Xóa khỏi giỏ hàng thành công";

            return RedirectToAction("Index");

        }
    }
}
