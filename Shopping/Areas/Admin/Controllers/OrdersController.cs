using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Repository;
using X.PagedList;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly DataContext _context;
        public INotyfService _notifyService { get; }


        public OrdersController(DataContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/OrderDetails
        public IActionResult Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 2;

            var dataContext = _context.Orders.Include(o => o.TransactStatus).Include(o => o.User).Include(o => o.Address).ToList();
            PagedList<OrderModel> models = new PagedList<OrderModel>(dataContext.AsQueryable(), pageNumber, pageSize);

            return View(models);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.Orders
                .Include(o => o.TransactStatus)
                .Include(o => o.User)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (orderModel == null)
            {
                return NotFound();
            }

            // Load order details for the selected order
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderModel.Id)
                .ToListAsync();

            var viewModel = new OrderDetailsViewModel
            {
                Order = orderModel,
                OrderDetails = orderDetails
            };

            return View(viewModel);
        }






        // GET: Admin/OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderModel = await _context.Orders.Include(x => x.TransactStatus).Include(x => x.Address).Include(x => x.User).FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderModel.Id);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatus, "Id", "Description", orderModel.TransactStatusId);

            return View(orderModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,OrderCode,Deleted,UserId,TransactStatusId,AddressId")] OrderModel orderModel)
        {
            if (id != orderModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderModel);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");

                    
                }
                catch (DbUpdateException)
                {
                    if (!OrderDetailModelExists(orderModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderModel.Id);
            return View(orderModel);
        }







        private bool OrderDetailModelExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
