using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Repository;
using System.Linq;

[Authorize]

public class OrderController : Controller
{
    private readonly DataContext _context;
    private readonly UserManager<AppUserModel> _userManager;

    public OrderController(DataContext context, UserManager<AppUserModel> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        string userId = _userManager.GetUserId(User);

        var orders = _context.Orders
            .Include(o => o.User)
            .Include(o => o.TransactStatus)
            .Include(o => o.User)
            .Include(o => o.Address)
            .Where(c => c.UserId == userId)
            .ToList();

        var orderViewModels = new List<OrderDetailsViewModel>();

        foreach (var order in orders)
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == order.Id)
                .ToList();

            var orderViewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = orderDetails
            };

            orderViewModels.Add(orderViewModel);
        }

        return View(orderViewModels);
    }

    public IActionResult ViewOrder(int Id)
    {
        var order = _context.Orders
            .Include(o => o.User)
            .Include(o => o.TransactStatus)
            .Include(o => o.User)
            .Include(o => o.Address)
            .FirstOrDefault(c => c.Id == Id);

        if (order == null)
        {
            return NotFound();
        }

        var orderDetails = _context.OrderDetails
            .Include(od => od.Product)
            .Include(od => od.Order)
            .Include(od => od.Order.Address)

            .Where(od => od.OrderId == Id)
            .ToList();

        var orderViewModel = new OrderDetailsViewModel
        {
            Order = order,
            OrderDetails = orderDetails
        };

        return View(orderViewModel);
    }

    public IActionResult Delete(int id)
    {
        var order = _context.Orders
            .Include(o => o.Address) 
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var address = order.Address;

        _context.Orders.Remove(order);

        if (address != null)
        {
            _context.Address.Remove(address);
        }

        _context.SaveChanges();
        TempData["message"] = "Deleted successfully";

        return RedirectToAction("Index");
    }




}
