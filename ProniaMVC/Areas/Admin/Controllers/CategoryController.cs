using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = "Admin,Moderator")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categoryVMs =await _context.Categories.Where(c=>!c.IsDeleted).Include(c=>c.Products).Select(c=>new GetCategoryAdminVM

            {
                Id=c.Id,
                Name = c.Name,
                ProductCount=c.Products.Count

            }

            ).ToListAsync();
           
            
            return View(categoryVMs);
        }

        public IActionResult Create()
        {
            CreateCategoryVM categoryVM= new CreateCategoryVM();
            return View(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM )
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == categoryVM.Name.Trim());
            if(result)
            {
                ModelState.AddModelError("Name", "Category already exists");
                return View(categoryVM);
            }
            Category category = new()
            {
                Name = categoryVM.Name

            };
            
           // categoryVM.CreatedAt=DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            
            Category category=await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            
            if(category == null) return NotFound();
            UpdateCategoryVM categoryVM = new()
            {
                Name=category.Name,
                Id=category.Id
            };

            return View(categoryVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,UpdateCategoryVM categoryVM)
        {
            if (id is null || id < 1) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (categoryVM is null) return NotFound();

            if(!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == categoryVM.Name.Trim() && c.Id!=id);
            if(result)
            {
                ModelState.AddModelError(nameof(categoryVM.Name),"category already exists");
                return View(categoryVM);

            }


            existed.Name=categoryVM.Name;
            _context.Categories.Update(existed);
            await _context.SaveChangesAsync();

             
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();
            category.IsDeleted = true;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index) );


        }
    }
}
