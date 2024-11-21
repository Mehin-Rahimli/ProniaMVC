﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
          
           
           
            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides.OrderBy(s => s.Order).Take(2).ToList(),
                Products=_context.Products.Include(p=>p.ProductImages).ToList(),
            };
            return View(homeVM);
        }
    }
}
