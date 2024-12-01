using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> sizes=await _context.Sizes.Where(s=>!s.IsDeleted).ToListAsync();
            var sizeVM=sizes.Select(s=> new GetSizeVM
            {
                Name = s.Name,
                Id= s.Id
            }).ToList();

            return View(sizeVM);
        }

        public IActionResult Create()
        {
            CreateSizeVM sizeVM= new CreateSizeVM();
            return View(sizeVM);

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if(!ModelState.IsValid)
            {
                return View(sizeVM);
            }


            bool result=await _context.Sizes.AnyAsync(s=>s.Name.Trim() == sizeVM.Name.Trim());
            if(result)
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View(sizeVM);
            }

            Size size = new()
            {
                Name = sizeVM.Name

            };

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if(id==null||id<1) return BadRequest();

            Size size=await _context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);

            if(size==null) return NotFound();

            UpdateSizeVM sizeVM = new()
            {
                Name = size.Name
            };
            return View(sizeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,UpdateSizeVM sizeVM)
        {
            if (id == null || id < 1) return BadRequest();

            
            if(!ModelState.IsValid)
            {
                return View(sizeVM);
            }

            Size existed=await _context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if (existed == null) return NotFound();

            bool result=await _context.Sizes.AnyAsync(s=>s.Name.Trim()==sizeVM.Name.Trim() && s.Id!=id);

            if(result)
            {
                ModelState.AddModelError(nameof(sizeVM), "Size already exists");
                return View(sizeVM);
            }

            existed.Name=sizeVM.Name;
            _context.Sizes.Update(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Size size= await _context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if (size == null) return NotFound();

            size.IsDeleted=true;
            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
