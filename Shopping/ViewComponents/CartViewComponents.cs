using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Repository;

using Microsoft.AspNetCore.Identity;
using System.Security.Claims; 

namespace Shopping.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;

        public CartSummaryViewComponent(DataContext context, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (User is ClaimsPrincipal claimsPrincipal)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
                                ?? new List<CartItemModel>();

                // Lọc ra chỉ những sản phẩm thuộc về UserId của người dùng hiện tại
                var userCartItems = cartItems.Where(c => c.UserId == userId).ToList();

                /*var itemCount = userCartItems.Sum(x => x.Quantity);
                return View(itemCount);*/

                var itemCount = userCartItems.Count;
                return View(itemCount);
            }

            return View(0);
        }
    }
}
