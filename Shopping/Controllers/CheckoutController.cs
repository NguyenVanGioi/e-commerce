using Microsoft.AspNetCore.Mvc;
using Shopping.Models.ViewModels;
using Shopping.Models;
using Shopping.Repository;

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Shopping.Services;

namespace Shopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly IVnPayService _vnPayservice;

        public CheckoutController(DataContext context, UserManager<AppUserModel> userManager, IVnPayService vnPayservice)
        {
            _context = context;
            _userManager = userManager;
            _vnPayservice = vnPayservice;

        }
        [HttpGet]
        public IActionResult Index()
        {
            // Lấy UserId của người dùng hiện tại
            string userId = _userManager.GetUserId(User);

            // Lấy thông tin giỏ hàng từ Session và lọc theo UserId
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
                                              ?.Where(c => c.UserId == userId)
                                              .ToList() ?? new List<CartItemModel>();

            // Tạo ViewModel và truyền thông tin giỏ hàng
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(CartItemViewModel cartVM, string paymentMethod)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (paymentMethod == "Thanh toán COD")
                {
                    // Lưu thông tin địa chỉ vào cơ sở dữ liệu
                    var address = new AddressModel
                    {
                        UserId = _userManager.GetUserId(User),
                        FullName = cartVM.FullName,
                        Email = cartVM.Email,
                        Phone = cartVM.Phone,
                        AddressOrder = cartVM.AddressOrder,
                        DateCreated = DateTime.Now
                    };

                    _context.Add(address);
                    _context.SaveChanges();

                    // Tiếp tục xử lý tạo đơn hàng
                    var orderItem = new OrderModel
                    {
                        OrderDate = DateTime.Now,
                        OrderCode = Guid.NewGuid().ToString(),
                        Deleted = false,
                        TransactStatusId = 1,
                        UserId = _userManager.GetUserId(User),
                        AddressId = address.Id
                    };

                    _context.Add(orderItem);
                    _context.SaveChanges();

                    // Lấy thông tin giỏ hàng từ Session
                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                    // Lưu thông tin chi tiết đơn hàng (sản phẩm trong giỏ hàng) vào cơ sở dữ liệu
                    foreach (var cartItem in cartItems.Where(x => x.UserId == orderItem.UserId))
                    {
                        var orderDetail = new OrderDetailModel
                        {
                            Quantity = cartItem.Quantity,
                            Thumb = cartItem.Thumb,
                            Total = cartItem.Total,
                            ShipDate = DateTime.Now,
                            OrderId = orderItem.Id,
                            ProductId = cartItem.Id
                        };

                        _context.Add(orderDetail);
                    }

                    // Xóa giỏ hàng sau khi đã tạo đơn hàng thành công
                    cartItems.RemoveAll(x => x.UserId == orderItem.UserId);
                    HttpContext.Session.SetJson("Cart", cartItems);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                    TempData["message"] = "Checkout successfully";

                }


                else if (paymentMethod == "Thanh toán VNPAY")
                {
                    // Lưu thông tin địa chỉ vào cơ sở dữ liệu trước khi chuyển hướng đến trang thanh toán VNPay
                    var address = new AddressModel
                    {
                        UserId = _userManager.GetUserId(User),
                        FullName = cartVM.FullName,
                        Email = cartVM.Email,
                        Phone = cartVM.Phone,
                        AddressOrder = cartVM.AddressOrder,
                        DateCreated = DateTime.Now
                    };

                    _context.Add(address);
                    _context.SaveChanges();
                    HttpContext.Session.SetJson("Address", address);

                    string userId = _userManager.GetUserId(User);

                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
                                                                  ?.Where(c => c.UserId == userId)
                                                                  .ToList() ?? new List<CartItemModel>();
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = cartItems.Sum(x => x.Quantity * x.Price),
                        CreatedDate = DateTime.Now,
                        Description = $"{cartVM.Phone}",
                        FullName = cartVM.FullName,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    HttpContext.Session.SetJson("Address", address);

                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                return View("Success");
            }
        }

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                // Giao dịch thất bại, xóa địa chỉ
                var addressFromSession = HttpContext.Session.GetJson<AddressModel>("Address");
                if (addressFromSession != null)
                {
                    _context.Remove(addressFromSession);
                    _context.SaveChanges();
                }
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return View("PaymentPail");
            }

            // Lưu đơn hàng và đơn chi tiết vào cơ sở dữ liệu
            var address = HttpContext.Session.GetJson<AddressModel>("Address");

            var orderItem = new OrderModel
            {
                OrderDate = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString(),
                Deleted = false,
                TransactStatusId = 1,
                UserId = _userManager.GetUserId(User),
                AddressId = address.Id
            };

            _context.Add(orderItem);
            _context.SaveChanges();

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            foreach (var cartItem in cartItems.Where(x => x.UserId == orderItem.UserId))
            {
                var orderDetail = new OrderDetailModel
                {
                    Quantity = cartItem.Quantity,
                    Thumb = cartItem.Thumb,
                    Total = cartItem.Total,
                    ShipDate = DateTime.Now,
                    OrderId = orderItem.Id,
                    ProductId = cartItem.Id
                };

                _context.Add(orderDetail);
            }

            // Xóa giỏ hàng sau khi đã tạo đơn hàng thành công
            cartItems.RemoveAll(x => x.UserId == orderItem.UserId);
            HttpContext.Session.SetJson("Cart", cartItems);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return View("Success");
        }

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }
        public IActionResult PaymentPail()
        {
            return View();
        }


    }
}



/*        [HttpPost]
        public async Task<IActionResult> Checkout(CartItemViewModel cartVM, string paymentMethod)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (paymentMethod == "COD")
                {
                    // Lưu thông tin địa chỉ vào cơ sở dữ liệu
                    var address = new AddressModel
                    {
                        UserId = _userManager.GetUserId(User),
                        FullName = cartVM.FullName,
                        Email = cartVM.Email,
                        Phone = cartVM.Phone,
                        AddressOrder = cartVM.AddressOrder,
                        DateCreated = DateTime.Now
                    };

                    _context.Add(address);
                    _context.SaveChanges();

                    // Tiếp tục xử lý tạo đơn hàng
                    var orderItem = new OrderModel
                    {
                        OrderDate = DateTime.Now,
                        OrderCode = Guid.NewGuid().ToString(),
                        Deleted = false,
                        TransactStatusId = 1,
                        UserId = _userManager.GetUserId(User),
                        AddressId = address.Id
                    };

                    _context.Add(orderItem);
                    _context.SaveChanges();

                    // Lấy thông tin giỏ hàng từ Session
                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                    // Lưu thông tin chi tiết đơn hàng (sản phẩm trong giỏ hàng) vào cơ sở dữ liệu
                    foreach (var cartItem in cartItems.Where(x => x.UserId == orderItem.UserId))
                    {
                        var orderDetail = new OrderDetailModel
                        {
                            Quantity = cartItem.Quantity,
                            Thumb = cartItem.Thumb,
                            Total = cartItem.Total,
                            ShipDate = DateTime.Now,
                            OrderId = orderItem.Id,
                            ProductId = cartItem.Id
                        };

                        _context.Add(orderDetail);
                    }

                    // Xóa giỏ hàng sau khi đã tạo đơn hàng thành công
                    cartItems.RemoveAll(x => x.UserId == orderItem.UserId);
                    HttpContext.Session.SetJson("Cart", cartItems);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                    TempData["message"] = "Checkout successfully";

                }


                else if (paymentMethod == "VNPAY")
                {

                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = cartItems.Sum(x => x.Quantity * x.Price),
                        CreatedDate = DateTime.Now,
                        Description = $"{cartVM.Phone}",
                        FullName = cartVM.FullName,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                return View("Success");
            }
        }


        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return View("PaymentPail");
            }


            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return View("Success");
        }*/

