using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Where(c => !c.IsDeleted).ToListAsync();

            var colorVM = colors.Select(t => new GetColorVM
            {
                Name = t.Name,
                Id = t.Id
            }).ToList();

            return View(colorVM);
        }

        public IActionResult Create()
        {
            CreateColorVM colorVM = new CreateColorVM();
            return View(colorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View(colorVM);
            }

            bool result = await _context.Colors.AnyAsync(c => c.Name.Trim() == colorVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Color already exists");
                return View(colorVM);
            }

            Color color = new()
            {
                Name = colorVM.Name


            };



            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color == null) return NotFound();

            UpdateColorVM colorVM = new()
            {
                Name = color.Name
            };
            return View(colorVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateColorVM colorVM)
        {
            if (id is null || id < 1) return BadRequest();


            if (!ModelState.IsValid)
            {
                return View(colorVM);
            }
            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);


            if (existed is null) return NotFound();

            bool result = await _context.Colors.AnyAsync(c => c.Name.Trim() == colorVM.Name.Trim() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(colorVM.Name), "Color already exists");
                return View(colorVM);

            }


            existed.Name = colorVM.Name;
            _context.Colors.Update(existed);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();
            color.IsDeleted = true;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

    