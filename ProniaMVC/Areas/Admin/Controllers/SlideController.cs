using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using System.Drawing;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            List<Slide>slides=await _context.Slides.ToListAsync();
            return View(slides);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? id,Slide slide)
        {
            //if(!slide.Photo.ContentType.Contains("image"/))
            //{
            //    ModelState.AddModelError("Photo", "File type is incorrect");
            //    return View();
            //}
            //if(slide.Photo.Length>2*1024*1024)
            //{
            //    ModelState.AddModelError("Photo", "File size must be less than 2 mb");
            //    return View();
            //}
            //return Content(slide.Photo.FileName);

            return Content(slide.Photo.FileName + " " + slide.Photo.ContentType + " " + slide.Photo.Length);

            if (!ModelState.IsValid) return View();

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }   
    }
}
