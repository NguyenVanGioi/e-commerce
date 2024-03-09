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

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
   

    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        public INotyfService _notifyService { get; }


        public CategoriesController(DataContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Thumb,Published,Alias,DateCreated,DateModified")] CategoryModel categoryModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {

                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(categoryModel.Name) + extension;
                    categoryModel.Thumb = await Utilities.UploadFile(fThumb, @"categories", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(categoryModel.Thumb)) categoryModel.Thumb = "default.jpg";
                categoryModel.Alias = Utilities.SEOUrl(categoryModel.Name);
                categoryModel.DateModified = DateTime.Now;
                categoryModel.DateCreated = DateTime.Now;

                _context.Add(categoryModel);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm thành công");

                return RedirectToAction(nameof(Index));
            }
            return View(categoryModel);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories.FindAsync(id);
            if (categoryModel == null)
            {
                return NotFound();
            }
            return View(categoryModel);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Thumb,Published,Alias,DateCreated,DateModified")] CategoryModel categoryModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != categoryModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string imageName = Utilities.SEOUrl(categoryModel.Name) + extension;
                        categoryModel.Thumb = await Utilities.UploadFile(fThumb, @"categories", imageName.ToLower());
                    }
                    if (string.IsNullOrEmpty(categoryModel.Thumb)) categoryModel.Thumb = "default.jpg";
                    categoryModel.Alias = Utilities.SEOUrl(categoryModel.Name);
                    categoryModel.DateModified = DateTime.Now;
                    _context.Update(categoryModel);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryModelExists(categoryModel.Id))
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
            return View(categoryModel);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryModel = await _context.Categories.FindAsync(id);
            if (categoryModel != null)
            {
                _context.Categories.Remove(categoryModel);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryModelExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
