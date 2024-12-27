using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Services.Interfaces;
using ProniaMVC.ViewModels;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace ProniaMVC.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBasketService _basketService;
        private readonly IEmailService _emailService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager, IBasketService basketService, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _basketService = basketService;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _basketService.GetBasketAsync());
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);

            if (!result) return NotFound();

            if (User.Identity.IsAuthenticated)
            {


                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if (item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count = 1
                    });
                }

                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];

                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                    }

                    else
                    {

                        basket.Add(new BasketCookieItemVM()
                        {
                            Id = id.Value,
                            Count = 1
                        });
                    }

                }
                else
                {
                    basket = new List<BasketCookieItemVM>();
                    basket.Add(new BasketCookieItemVM()
                    {
                        Id = id.Value,
                        Count = 1

                    });
                }
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("basket", json);
            }

            return RedirectToAction(nameof(GetBasket));

        }

        public async Task<IActionResult> GetBasket()
        {
            return PartialView("BasketPartialView", await _basketService.GetBasketAsync());
        }


        [HttpPost]
        public async Task<IActionResult> IncreaseCount(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);

            if (!result) return NotFound();
            if (User.Identity.IsAuthenticated)
            {


                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if (item is not null)
                {
                    item.Count++;
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];

                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                        string json = JsonConvert.SerializeObject(basket);
                        Response.Cookies.Append("basket", json);
                    }
                }
            }

            return RedirectToAction(nameof(GetBasket));
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseCount(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);

            if (!result) return NotFound();
            if (User.Identity.IsAuthenticated)
            {


                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem? item = user?.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if (item is not null)
                {
                    if (item.Count > 1)
                    {
                        item.Count--;

                    }
                    else
                    {
                        user.BasketItems.Remove(item);

                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];

                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM? existed = basket?.FirstOrDefault(b => b.Id == id);
                    if (existed is not null)
                    {
                        if (existed.Count > 1)
                        {
                            existed.Count--;

                        }
                        else
                        {
                            basket.Remove(existed);

                        }
                        string json = JsonConvert.SerializeObject(basket);
                        Response.Cookies.Append("basket", json);
                    }
                }
            }

            return RedirectToAction(nameof(GetBasket));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);

            if (!result) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user is null) return Unauthorized();
                BasketItem? item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if (item is not null)
                {
                    user.BasketItems.Remove(item);
                    await _context.SaveChangesAsync();
                }

            }
            else
            {
                List<BasketCookieItemVM> basket;

                string? cookies = Request.Cookies["basket"];

                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM? existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        basket.Remove(existed);
                        string json = JsonConvert.SerializeObject(basket);
                        Response.Cookies.Append("basket", json);
                    }
                }
            }

            return RedirectToAction(nameof(GetBasket));

        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {
            OrderVM orderVM = new()
            {
                BasketInOrderVMs = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Price = bi.Product.Price,
                    Name = bi.Product.Name,
                    Subtotal = bi.Product.Price * bi.Count
                }).ToListAsync()
            };

            return View(orderVM);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {

            AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
            .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<BasketItem> basketItems = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(bi => bi.Product)
                .ToListAsync();


            if (!ModelState.IsValid)
            {
                orderVM.BasketInOrderVMs = basketItems.Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Price = bi.Product.Price,
                    Name = bi.Product.Name,
                    Subtotal = bi.Product.Price * bi.Count
                }).ToList();
                return View(orderVM);
            }

            Order order = new()
            {
                Address = orderVM.Address,
                Status = null,
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsDeleted = false,
                OrderItems = basketItems.Select(bi => new OrderItem
                {
                    AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Count = bi.Count,
                    Price = bi.Product.Price,
                    ProductId = bi.ProductId
                }).ToList(),
                TotalPrice = basketItems.Sum(bi => bi.Product.Price * bi.Count)
            };


            await _context.AddAsync(order);
            _context.BasketItems.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            string body = @"
                Your order successfully placed:
                <table border=""1"">
                     <thead>
                         <tr>
                              <th>Name</th>
                             <th>Price</th>
                             <th>Count</th>
                         </tr>
                     </thead>
                     <tbody>";
            foreach (var item in order.OrderItems)
            {
                body += @$" <tr>
                              <td>{item.Product.Name}</td>
                              <td>{item.Price}</td>
                              <td>{item.Count}</td>
                            </tr>";
            }
            body += @"</tbody>
                   </table>";
            await _emailService.SendMailAsync(user.Email, "Your Order",body,true);
            return RedirectToAction("Index", "Home");
        }
    }
}