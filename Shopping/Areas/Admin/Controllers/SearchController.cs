using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Repository;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    

    public class SearchController : Controller
    {
        private readonly DataContext _context;
        public SearchController(DataContext context)
        {
            _context = context;
        }


        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            try
            {
                List<ProductModel> ls = new List<ProductModel>();

                if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
                {
                    // Nếu từ khóa trống, trả về toàn bộ danh sách sản phẩm của trang index
                    ls = _context.Products
                        .AsNoTracking()
                        .Include(a => a.Category)
                        .Include(a => a.Brand)
                        .OrderBy(x => x.Id)
                        .Take(4)
                        .ToList();

                }
                else
                {
                    // Ngược lại, thực hiện tìm kiếm theo từ khóa
                    ls = _context.Products
                        .AsNoTracking()
                        .Include(a => a.Category)
                        .Include(a => a.Brand)
                        .Where(x => x.Name.Contains(keyword))
                        .OrderBy(x => x.Id)
                        .ToList();

                }

                if (ls.Any())
                {
                    return PartialView("ListProductsSearchPartial", ls);
                }
                else
                {
                    return PartialView("_EmptyPartial");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug
                Console.WriteLine(ex.Message);
                return PartialView("_EmptyPartial");
            }
        }




    }
}
