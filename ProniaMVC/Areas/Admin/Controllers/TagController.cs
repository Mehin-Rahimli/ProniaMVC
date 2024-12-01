using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Where(c => !c.IsDeleted).ToListAsync();

            var tagVM=tags.Select(t=>new GetTagAdminVM
            {
                Name = t.Name,
                Id = t.Id
            }).ToList();

            return View(tagVM);
        }

        public IActionResult Create()
        {
            CreateTagVM tagVM = new CreateTagVM();
            return View(tagVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View(tagVM);
            }

            bool result = await _context.Tags.AnyAsync(c => c.Name.Trim() == tagVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Tag already exists");
                return View(tagVM);
            }

            Tag tag = new()
            {
                Name = tagVM.Name
               
                
            };



            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (tag == null) return NotFound();

            UpdateTagVM tagVM = new()
            {
                Name = tag.Name
            };
            return View(tagVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateTagVM tagVM)
        {
            if (id is null || id < 1) return BadRequest();

          
            if (!ModelState.IsValid)
            {
                return View(tagVM);
            } 
            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id); 
            

            if (existed is null) return NotFound();
            
            bool result = await _context.Tags.AnyAsync(c => c.Name.Trim() == tagVM.Name.Trim() && c.Id != id);
           
            if (result)
            {
                ModelState.AddModelError(nameof(tagVM.Name), "Tag already exists");
                return View(tagVM);

            }


            existed.Name = tagVM.Name;
            _context.Tags.Update(existed);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (tag is null) return NotFound();
            tag.IsDeleted = true;

           _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

