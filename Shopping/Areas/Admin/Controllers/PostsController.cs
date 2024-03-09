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
   

    public class PostsController : Controller
    {
        private readonly DataContext _context;
        public INotyfService _notifyService { get; }


        public PostsController(DataContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contents,Thumb,Published,Alias,DateCreated,Active")] PostModel postModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(postModel.Title) + extension;
                    postModel.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(postModel.Thumb)) postModel.Thumb = "default.jpg";
                postModel.Alias = Utilities.SEOUrl(postModel.Title);
                postModel.DateCreated = DateTime.Now;
                _context.Add(postModel);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm Tin thành công");

                return RedirectToAction(nameof(Index));
            }
            return View(postModel);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            return View(postModel);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Contents,Thumb,Published,Alias,DateCreated,Active")] PostModel postModel, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != postModel.Id)
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
                        string imageName = Utilities.SEOUrl(postModel.Title) + extension;
                        postModel.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                    }
                    if (string.IsNullOrEmpty(postModel.Thumb)) postModel.Thumb = "default.jpg";
                    postModel.Alias = Utilities.SEOUrl(postModel.Title);
                    _context.Update(postModel);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Sửa Tin thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.Id))
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
            return View(postModel);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel != null)
            {
                _context.Posts.Remove(postModel);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa Tin thành công");

            return RedirectToAction(nameof(Index));
        }

        private bool PostModelExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
