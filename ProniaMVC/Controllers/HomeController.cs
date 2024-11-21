using Microsoft.AspNetCore.Mvc;
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
            //_context=new AppDbContext();
                //AppDbContext appDb = new AppDbContext();
        }
        public IActionResult Index()
        {
           

            //_context.Slides.AddRange(slides);
            //_context.SaveChanges
           
            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides.OrderBy(s => s.Order).Take(2).ToList()
            };
            return View(homeVM);
        }
    }
}
