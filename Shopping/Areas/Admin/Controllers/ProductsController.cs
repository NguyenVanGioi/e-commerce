using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Helper;
using Shopping.Models;
using Shopping.Repository;
using X.PagedList;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
   

    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        public INotyfService _notifyService { get; }


        public ProductsController(DataContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Products
        /*public async Task<IActionResult> Index()
        {
            var dataContext = _context.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(await dataContext.ToListAsync());
        }*/

        public IActionResult Index(int page = 1, int CatID = 0, int BrandID = 0)
        {
            var dataContext = _context.Products.Include(p => p.Brand).Include(p => p.Category);
            var pageNumber = page;
            var pageSize = 5;
            List<ProductModel> lsProducts = new List<ProductModel>();
            if (CatID != 0)
            {
                lsProducts = _context.Products.AsNoTracking().Where(x => x.CategoryId == CatID).Include(x => x.Brand).Include(x => x.Category).OrderByDescending(x => x.Id).ToList();

            }
            else
            {
                lsProducts = _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Brand).OrderBy(x => x.Id).ToList();

            }

            if (BrandID != 0)
            {
                lsProducts = lsProducts.Where(x => x.BrandId == BrandID).ToList();
            }
            PagedList<ProductModel> models = new PagedList<ProductModel>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentBrandID = BrandID;

            ViewBag.CurrentPage = pageNumber;

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", CatID);
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", BrandID);

            return View(models);
        }

        public IActionResult Filtter(int CatID = 0, int BrandID = 0)
        {
            var url = $"/Admin/Products?CatID={CatID}&BrandID={BrandID}";
            if (CatID == 0 && BrandID == 0)
            {
                url = $"/Admin/Products";
            }
            return Json(new { status = "success", redirectUrl = url });
        }


        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Thumb,Active,Alias,UnitsInstock,CategoryId,BrandId,DateCreated,DateModified")] ProductModel productModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                productModel.Name = Utilities.ToTitleCase(productModel.Name);

                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(productModel.Name) + extension;
                    productModel.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(productModel.Thumb)) productModel.Thumb = "default.jpg";

                productModel.Alias = Utilities.SEOUrl(productModel.Name);
                productModel.DateModified = DateTime.Now;
                productModel.DateCreated = DateTime.Now;
                _context.Add(productModel);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm sản phẩm thành công");

                return RedirectToAction(nameof(Index));
            }
           else
{
    var errors = ModelState.Values.SelectMany(v => v.Errors);
    foreach (var error in errors)
    {
        Console.WriteLine(error.ErrorMessage);
    }
}
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", productModel.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", productModel.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Thumb,Active,Alias,UnitsInstock,CategoryId,BrandId,DateCreated,DateModified")] ProductModel productModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productModel.Name = Utilities.ToTitleCase(productModel.Name);

                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(productModel.Name) + extension;
                        productModel.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(productModel.Thumb)) productModel.Thumb = "default.jpg";

                    productModel.Alias = Utilities.SEOUrl(productModel.Name);
                    productModel.DateModified = DateTime.Now;
                    _context.Update(productModel);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", productModel.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", productModel.CategoryId);
            return View(productModel);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel != null)
            {
                _context.Products.Remove(productModel);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
