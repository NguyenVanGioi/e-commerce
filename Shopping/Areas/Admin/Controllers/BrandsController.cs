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
    

    public class BrandsController : Controller
    {
        private readonly DataContext _context;
        public INotyfService _notifyService { get; }


        public BrandsController(DataContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        // GET: Admin/Brands
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }

        // GET: Admin/Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandModel = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandModel == null)
            {
                return NotFound();
            }

            return View(brandModel);
        }

        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Thumb,Published,Alias,DateCreated,DateModified")] BrandModel brandModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(brandModel.Name) + extension;
                    brandModel.Thumb = await Utilities.UploadFile(fThumb, @"brands", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(brandModel.Thumb)) brandModel.Thumb = "default.jpg";
                brandModel.Alias = Utilities.SEOUrl(brandModel.Name);
                brandModel.DateModified = DateTime.Now;
                brandModel.DateCreated = DateTime.Now;
                _context.Add(brandModel);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm thành công");

                return RedirectToAction(nameof(Index));
            }
            return View(brandModel);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandModel = await _context.Brands.FindAsync(id);
            if (brandModel == null)
            {
                return NotFound();
            }
            return View(brandModel);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Thumb,Published,Alias,DateCreated,DateModified")] BrandModel brandModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != brandModel.Id)
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
                        string imageName = Utilities.SEOUrl(brandModel.Name) + extension;
                        brandModel.Thumb = await Utilities.UploadFile(fThumb, @"brands", imageName.ToLower());
                    }
                    if (string.IsNullOrEmpty(brandModel.Thumb)) brandModel.Thumb = "default.jpg";
                    brandModel.Alias = Utilities.SEOUrl(brandModel.Name);
                    brandModel.DateModified = DateTime.Now;
                    _context.Update(brandModel);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandModelExists(brandModel.Id))
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
            return View(brandModel);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandModel = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandModel == null)
            {
                return NotFound();
            }

            return View(brandModel);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandModel = await _context.Brands.FindAsync(id);
            if (brandModel != null)
            {
                _context.Brands.Remove(brandModel);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");

            return RedirectToAction(nameof(Index));
        }

        private bool BrandModelExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
