using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Repository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;

namespace Shopping.ViewComponents
{
    public class OrderSummaryViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;

        public OrderSummaryViewComponent(DataContext context, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            if (User is ClaimsPrincipal claimsPrincipal)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);

                int orderCount = _context.Orders.Count(o => o.UserId == userId);

                return View(orderCount);
            }

            return View(0);
        }
    }
}
