using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;

namespace ProniaMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    // [Authorize(Roles = "Admin,Moderator")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var ordersInfoVM = await _context.OrdersInfos
                .Include(oi => oi.AppUser)
                .Select(oi => new OrdersInfoVM
                {
                    OrderNo = oi.OrderNo,
                    UserName = oi.AppUser.UserName,
                    TotalPrice = oi.TotalPrice,
                    Date = DateTime.Parse(oi.Date).ToString("MM/dd/yyyy"),
                    Status = oi.Status


                }).ToListAsync();
            return View(ordersInfoVM);
        }

        [HttpPost]
        public async Task<IActionResult> OrdersTable()
        {
            return RedirectToAction("Index");
        }
    }
}
