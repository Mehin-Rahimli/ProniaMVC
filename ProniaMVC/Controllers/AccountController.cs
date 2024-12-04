using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaMVC.Models;
using ProniaMVC.Utilities.Extensions;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string formattedName = userVM.Name.CheckName();
           
            if (formattedName == null)
            {
                ModelState.AddModelError(nameof(userVM.Name),
                "Name is not correct");
               

            }

            string formattedSurname = userVM.Surname.CheckName();
            if (formattedSurname == null)
            {
                ModelState.AddModelError(nameof(userVM.Surname),
                "Surname is not correct");
                

            }

            if (!userVM.Email.CheckEmail())
            {
                ModelState.AddModelError(nameof(userVM.Email), "Email is not correct");
                return View();
            }


            AppUser user = new AppUser
            {
                Name = formattedName,
                Surname = formattedSurname,
                Email = userVM.Email,
                UserName = userVM.Name

            };

            IdentityResult result = await _userManager.CreateAsync(user, userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(HomeController.Index), "Home");



        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
