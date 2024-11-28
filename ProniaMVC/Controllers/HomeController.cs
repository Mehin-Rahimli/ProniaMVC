using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class HomeController : Controller
    {
        public readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {

           _context=context;
           
        }
        public async Task<IActionResult> Index()
        {
          
           
           
            HomeVM homeVM = new HomeVM
            {
                Slides =await _context.Slides
                .OrderBy(s => s.Order)
                .Take(7)
                .ToListAsync(),
                
                

                Products=await _context.Products
                .Take(8)
                .Include(p=>p.ProductImages
                .Where(pi=>pi.IsPrimary!=null))
                .ToListAsync(),
            };
            return View(homeVM);
        }
    }
}
