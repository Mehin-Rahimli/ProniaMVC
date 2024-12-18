using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilities.Enums;
using ProniaMVC.Utilities.Exceptions;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,int? categoryId,int key=1,int page=1)
        {
            IQueryable<Product> query = _context.Products.Include(q=>q.ProductImages.Where(p=>p.IsPrimary!=null));
            if (!string.IsNullOrEmpty(search))
            {
                query=query.Where(p=>p.Name.ToLower().Contains(search.ToLower()));
            }

            if(categoryId != null && categoryId > 0)
            {
                query=query.Where(p=>p.CategoryId==categoryId);
            }

            switch (key)
            {
                case (int)SortType.Name:
                    query=query.OrderBy(q=>q.Name);
                    break;

                case (int)SortType.Price:
                    query = query.OrderByDescending(q => q.Price);
                    break;

                case (int)SortType.Date:
                    query = query.OrderByDescending(q => q.CreatedAt);
                    break;
                default:
                    break;
            }

            int count = query.Count();
            double total = Math.Ceiling((double)count/3);

            query = query.Skip((page - 1) * 3).Take(3);


            ShopVM shopVM = new ShopVM
            {
                Products = await query.Select(p => new GetProductVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                    SecondaryImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false).Image,
                    Price = p.Price

                }).ToListAsync(),
                Categories = await _context.Categories.Select(c=> new GetCategoryVM
                {
                    Id=c.Id,
                    Name = c.Name,
                    Count=c.Products.Count

                }).ToListAsync(),
                Search = search,
                CategoryId = categoryId,
                Key = key,
                TotalPage = total,
                CurrentPage = page,
            };
            return View(shopVM);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null && id < 1) throw new BadRequestException($"{id} is wrong");

            Product? product =await _context.Products
                .Include(p=>p.ProductImages
                .OrderByDescending(pi=>pi.IsPrimary))
                .Include(p=>p.Category)
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(ps=>ps.Size)
                .FirstOrDefaultAsync(p=>p.Id==id);

            if (product == null) throw new NotFoundException($"{id} can not be found");



            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts =await  _context.Products
                .Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Include(p=>p.ProductImages
                .Where(pi=>pi.IsPrimary!=null))
                .Take(8)
                .ToListAsync()
            };

            return View(detailVM);
        }
    }
}
