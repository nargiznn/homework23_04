using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
	public class AccountController:Controller
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
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser user = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FullName = registerVM.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string returnUrl)
        {
            AppUser user = await _userManager.FindByEmailAsync(loginVM.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View(loginVM);
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
        }

        //public async Task<IActionResult> GetNameAsync()
        //{
        //    var username = User.Identity.Name;
        //    return Json(username);
        //}

        public IActionResult GetName()
        {
            var username = User.Identity.Name;
            return Json(User.Identity);
        }
    }
}
