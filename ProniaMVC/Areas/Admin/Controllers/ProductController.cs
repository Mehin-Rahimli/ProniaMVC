using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilities.Enums;
using ProniaMVC.Utilities.Extensions;

namespace ProniaMVC.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var productsVMs = await _context.Products
                 .Include(p => p.Category)
                 .Include(p => p.ProductImages
                 .Where(pi => pi.IsPrimary == true))
                 .Select(p =>
                 new GetProductAdminVM
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Price = p.Price,
                     CategoryName = p.Category.Name,
                     Image = p.ProductImages[0].Image
                 }
                 )
                 .ToListAsync();




            return View(productsVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Colors=await _context.Colors.Where(c=>!c.IsDeleted).ToListAsync(),
                Sizes = await _context.Sizes.Where(s => !s.IsDeleted).ToListAsync()
            };

            return View(productVM);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();

            productVM.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            productVM.Colors=await _context.Colors.Where(c=>!c.IsDeleted).ToListAsync();
            productVM.Sizes = await _context.Sizes.Where(s => !s.IsDeleted).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }


            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "File type is incorrect");
                return View(productVM);
            }


            if (!productVM.MainPhoto.ValidateSize(FileSize.MB,1))
            {
                ModelState.AddModelError("MainPhoto", "File size is incorrect");
                return View(productVM);
            }


            if (!productVM.HoverPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "File type is incorrect");
                return View(productVM);
            }


            if (!productVM.HoverPhoto.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError("HoverPhoto", "File size is incorrect");
                return View(productVM);
            }


            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category doesn't exist");
                return View(productVM);
            }

            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));

                if (tagResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
                    return View(productVM);
                }
            }


            if(productVM.ColorIds is not null)
            {
                bool colorResult=productVM.ColorIds.Any(cId=>!productVM.Colors.Exists(c => c.Id == cId));
                if(colorResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.ColorIds), "Colors are wrong");
                    return View(productVM);
                }
            }

            if (productVM.SizeIds is not null)
            {
                bool sizeResult = productVM.SizeIds.Any(sId => !productVM.Sizes.Exists(s=> s.Id == sId));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.SizeIds), "Sizes are wrong");
                    return View(productVM);
                }
            }

            ProductImage main = new()
            {
                Image=await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images"),
                IsPrimary=true,
                CreatedAt=DateTime.Now,
                IsDeleted=false


            };

            ProductImage hover = new()
            {
                Image = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = false,
                CreatedAt = DateTime.Now,
                IsDeleted = false


            };

            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId.Value,
                Description = productVM.Description,
                Price = productVM.Price.Value,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ProductImages=new List<ProductImage> { main ,hover}

            };

            if (productVM.TagIds is not null)
            {
                product.ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList();
            }


            if(productVM.ColorIds is not null)
            {
                product.ProductColors=productVM.ColorIds.Select(cId=>new ProductColor { ColorId = cId }).ToList();
            }


            if (productVM.SizeIds is not null)
            {
                product.ProductSizes = productVM.SizeIds.Select(sId => new ProductSize { SizeId = sId }).ToList();
            }



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductTags).Include(p => p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
           
            if (product == null) return NotFound();
            UpdateProductVM productVM = new()
            {
                Name = product.Name,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Description = product.Description,
                Price = product.Price,
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync(),
                ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList(),
                Colors =await _context.Colors.Where(c=>!c.IsDeleted).ToListAsync(),
                SizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList(),
                Sizes = await _context.Sizes.Where(s => !s.IsDeleted).ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null || id < 1) return BadRequest();

            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            productVM.Colors=await _context.Colors.Where(c=>!c.IsDeleted).ToListAsync();
            productVM.Sizes = await _context.Sizes.Where(s => !s.IsDeleted).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            Product existed = await _context.Products.Include(p => p.ProductTags).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);

            if (existed == null) return NotFound();



            if (existed.CategoryId != productVM.CategoryId)
            {
                bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
                if (!result)
                {
                    return View(productVM);
                }
            }



            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
                if (tagResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.TagIds), "Tags are wrong");
                    return View(productVM);
                }
            }

            if (productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }
            else
            {
                productVM.TagIds = productVM.TagIds.Distinct().ToList();
            }





            if (productVM.ColorIds is not null)
            {
                bool colorResult = productVM.ColorIds.Any(cId => !productVM.Colors.Exists(c => c.Id == cId));
                if (colorResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.ColorIds), "colors are wrong");
                    return View(productVM);
                }
            }

            if (productVM.ColorIds is null)
            {
                productVM.ColorIds = new();
            }
            else
            {
                productVM.ColorIds = productVM.ColorIds.Distinct().ToList();
            }


            if (productVM.SizeIds is not null)
            {
                bool sizeResult = productVM.SizeIds.Any(sId => !productVM.Sizes.Exists(s => s.Id == sId));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SizeIds), "Sizes are wrong");
                    return View(productVM);
                }
            }

            if (productVM.SizeIds is null)
            {
                productVM.SizeIds = new();
            }
            else
            {
                productVM.SizeIds = productVM.SizeIds.Distinct().ToList();
            }



            _context.ProductTags.RemoveRange(existed.ProductTags
                .Where(pTag => !productVM.TagIds
                .Exists(tId => tId == pTag.TagId))
                .ToList());

            _context.ProductTags.AddRange(productVM.TagIds
                .Where(tId => !existed.ProductTags
                .Exists(pTag => pTag.TagId == tId))
                .ToList()
                .Select(tId => new ProductTag
                { TagId = tId, ProductId = existed.Id }));





            _context.ProductColors.RemoveRange(existed.ProductColors
             .Where(pColor => !productVM.ColorIds
             .Exists(tId => tId == pColor.ColorId))
             .ToList());

            _context.ProductColors.AddRange(productVM.ColorIds
                .Where(cId => !existed.ProductColors
                .Exists(pColor => pColor.ColorId == cId))
                .ToList()
                .Select(cId => new ProductColor
                { ColorId = cId, ProductId = existed.Id }));




            _context.ProductSizes.RemoveRange(existed.ProductSizes
             .Where(pSize => !productVM.SizeIds
             .Exists(sId => sId == pSize.SizeId))
             .ToList());

            _context.ProductSizes.AddRange(productVM.SizeIds
                .Where(sId => !existed.ProductSizes
                .Exists(pSize => pSize.SizeId == sId))
                .ToList()
                .Select(sId => new ProductSize
                { SizeId = sId, ProductId = existed.Id }));




            existed.SKU = productVM.SKU;
            existed.Price = productVM.Price.Value;
            existed.CategoryId = productVM.CategoryId.Value;
            existed.Description = productVM.Description;
            existed.Name = productVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
